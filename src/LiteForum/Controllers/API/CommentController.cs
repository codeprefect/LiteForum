using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LiteForum.Entities.Models;
using LiteForum.Helpers;
using LiteForum.Models;
using LiteForum.Services.Interfaces;
using LiteForum.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LiteForum.Controllers.API
{
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
        public async Task<IActionResult> Get(int postId)
        {
            var Comments = await _comments.GetAsync(filter: c => c.PostId == postId);
            return Ok(Comments.Select(c => c.ToVModel()));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSingle(int postId, int id, bool withChild = false)
        {
            Expression<Func<Comment, bool>> filter = c => c.PostId == postId && c.Id == id;
            var comment = withChild ?
                await _comments.GetOneAsync(filter: filter, includeProperties: "Replies") :
                await _comments.GetOneAsync(filter: filter);
            if (comment == null) return NotFound();
            return Ok(comment?.ToVModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(int postId, [FromBody] CommentVModel comment)
        {
            var newComment = comment.ToModel();
            newComment.PostId = postId;
            newComment.UserId = UserId;
            newComment = _comments.Create(newComment, UserId);
            await _comments.SaveAsync();
            _logger.LogInformation($"User: {UserId} created a new comment {newComment}");
            return Created(Request.Path.Value, newComment.ToVModel());
        }

        [HttpPut]
        public async Task<IActionResult> Update(int postId, [FromBody] CommentVModel comment)
        {
            var oldComment = await _comments.GetByIdAsync(comment.Id);
            if (oldComment.UserId != UserId) throw new UnauthorizedAccessException(userMismatchMessage);
            if (oldComment.PostId != postId) throw new AccessViolationException(postMismatchMessage);
            oldComment.Content = comment.Content;
            _comments.Update(oldComment, UserId);
            await _comments.SaveAsync();
            _logger.LogInformation($"User: {UserId} modified a his comment {oldComment}");
            return Ok(oldComment.ToVModel());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int postId, int id)
        {
            var comment = await _comments.GetByIdAsync(id);
            if (comment.UserId != UserId) throw new UnauthorizedAccessException(userMismatchMessage);
            if (comment.PostId != postId) throw new AccessViolationException(postMismatchMessage);
            _comments.Delete(id);
            await _comments.SaveAsync();
            _logger.LogInformation($"User: {UserId} deleted his comment with id: {id}");
            return Ok(new LiteForumResponseMessage(200, "deleted successfully"));
        }

        #region Helpers
        private const string userMismatchMessage = "comment was authored by another user.";
        private const string postMismatchMessage = "comment does not belong to the specified post.";
        #endregion
    }
}
