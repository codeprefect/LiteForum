using System;
using System.Linq.Expressions;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LiteForum.Entities.Models;
using LiteForum.Helpers;
using LiteForum.Models;
using LiteForum.Services.Interfaces;
using LiteForum.ViewModels;

namespace LiteForum.Controllers.API
{
    [Authorize(policy: "Authenticated")]
    [Route("api/post/{postId}/[controller]")]
    public class CommentController : BaseApiController
    {
        private readonly ILogger<CommentController> _logger;
        private readonly IDataService<LiteForumDbContext, Comment> _comments;

        public CommentController(ILogger<CommentController> logger,
            IDataService<LiteForumDbContext, Comment> comments)
        {
            _logger = logger;
            _comments = comments;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            try
            {
                var Comments = await _comments.GetAsync(filter: filter);
                return Ok(Comments.Select(c=> c.ToVModel()));
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to fetch comments due to {e.Message ?? e.InnerException.Message}");
                return BadRequest(e.ToResponse(500));
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSingle(int id, bool withChild = false)
        {
            if (id <= 0) return BadRequest($"submitted id: {id} is not valid");

            Expression<Func<Comment, bool>> filter = p => p.PostId == _postId && p.Id == id;
            try
            {
                var comment = withChild ?
                    await _comments.GetOneAsync(filter: filter, includeProperties: "Replies") :
                    await _comments.GetOneAsync(filter: filter);

                return Ok(comment.ToVModel());
            }
            catch (Exception e)
            {
                _logger.LogError($"failed to get comment with id: {id}, due to {e.Message ?? e.InnerException.Message}");
                return BadRequest(e.ToResponse(500));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CommentVModel comment)
        {
            if (!ModelState.IsValid) return BadRequest($"submitted comment has {ModelState.ErrorCount} errors");

            try
            {
                var newComment = comment.ToModel();
                newComment.PostId = _postId;
                newComment.UserId = UserId;
                _comments.Create(newComment, UserId);
                await _comments.SaveAsync();
                _logger.LogInformation($"User: {UserId} created a new comment {newComment}");
                return Created(Request.Path.Value, newComment.ToVModel());
            }
            catch (Exception e)
            {
                _logger.LogError($"comment creation by {UserId} failed due to {e.Message ?? e.InnerException.Message}");
                return BadRequest(e.ToResponse(500));
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody]CommentVModel comment)
        {
            if (!ModelState.IsValid) return BadRequest($"submitted Comment has {ModelState.ErrorCount} errors");

            try
            {
                var oldComment = await _comments.GetByIdAsync(comment.Id);
                if (oldComment.UserId != UserId) throw new Exception("you do not have write access to this comment");
                if (oldComment.PostId != _postId) throw new Exception("you tried to mangle the system. Thanks");

                oldComment.Content = comment.Content;
                _comments.Update(oldComment, UserId);
                await _comments.SaveAsync();
                _logger.LogInformation($"User: {UserId} modified a his comment {oldComment}");
                return Ok(oldComment.ToVModel());
            }
            catch (Exception e)
            {
                _logger.LogError($"comment modification by {UserId} failed due to {e.Message ?? e.InnerException.Message}");
                return BadRequest(e.ToResponse(500));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest("submitted id: ${id} is invalid");

            try
            {
                var comment = await _comments.GetByIdAsync(id);
                if (comment.UserId != UserId) throw new Exception("sorry, you cannot delete this comment.");
                if (comment.PostId != _postId) throw new Exception("you tried to mangle the system. Thanks");

                _comments.Delete(id);
                await _comments.SaveAsync();
                _logger.LogInformation($"User: {UserId} deleted his comment with id: {id}");
                return Ok(new LiteForumResponseMessage(200, "deleted successfully"));
            }
            catch (Exception e)
            {
                _logger.LogError($"comment deletion by {UserId} failed due to {e.Message ?? e.InnerException.Message}");
                return BadRequest(e.ToResponse(500));
            }
        }

        #region Helpers
        private int _postId
        {
            get => Convert.ToInt32(RouteData.Values["postId"]);
        }

        protected Expression<Func<Comment, bool>> filter {
            get => p => p.PostId == _postId;
        }

        #endregion

    }
}
