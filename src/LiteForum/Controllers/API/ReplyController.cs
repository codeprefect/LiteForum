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
    [Route("api/comment/{commentId}/[controller]")]
    [ApiController]
    public class ReplyController : BaseApiController
    {
        private readonly ILogger<ReplyController> _logger;
        private readonly IDataService<LiteForumDbContext, Reply> _replies;

        public ReplyController(ILogger<ReplyController> logger,
            IDataService<LiteForumDbContext, Reply> replies)
        {
            _logger = logger;
            _replies = replies;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            try
            {
                var Replys = await _replies.GetAsync(filter: filter);
                return Ok(Replys.Select(c => c.ToVModel()));
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to fetch replies due to {e.Message ?? e.InnerException.Message}");
                return BadRequest(e.ToResponse(500));
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSingle(int id)
        {
            if (id <= 0) return BadRequest($"submitted id: {id} is not valid");

            Expression<Func<Reply, bool>> filter = p => p.Id == id && p.CommentId == _commentId;

            try
            {
                var reply = await _replies.GetOneAsync(filter: filter);
                return Ok(reply.ToVModel());
            }
            catch (Exception e)
            {
                _logger.LogError($"failed to get reply with id: {id}, due to {e.Message ?? e.InnerException.Message}");
                return BadRequest(e.ToResponse(500));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]ReplyVModel reply)
        {
            try
            {
                var newReply = reply.ToModel();
                newReply.CommentId = _commentId;
                newReply.UserId = UserId;
                newReply = _replies.Create(newReply, UserId);
                await _replies.SaveAsync();
                _logger.LogInformation($"User: {UserId} created a new reply {newReply}");
                return Created(Request.Path.Value, newReply.ToVModel());
            }
            catch (Exception e)
            {
                _logger.LogError($"reply creation by {UserId} failed due to {e.Message ?? e.InnerException.Message}");
                return BadRequest(e.ToResponse(500));
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody]ReplyVModel reply)
        {
            try
            {
                var oldReply = await _replies.GetByIdAsync(reply.Id);
                if (oldReply.UserId != UserId) throw new Exception("you do not have write access to this reply");
                if (oldReply.CommentId != _commentId) throw new Exception("you tried to mangle the system. Thanks");

                oldReply.Content = reply.Content;
                oldReply = _replies.Update(oldReply, UserId);
                await _replies.SaveAsync();
                _logger.LogInformation($"User: {UserId} modified a his reply {oldReply}");
                return Ok(oldReply.ToVModel());
            }
            catch (Exception e)
            {
                _logger.LogError($"reply modification by {UserId} failed due to {e.Message ?? e.InnerException.Message}");
                return BadRequest(e.ToResponse(500));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest("submitted id: ${id} is invalid");

            try
            {
                var reply = await _replies.GetByIdAsync(id);
                if (reply.UserId != UserId) throw new Exception("sorry, you cannot delete this reply.");
                if (reply.CommentId != _commentId) throw new Exception("you tried to mangle the system. Thanks");

                _replies.Delete(id);
                await _replies.SaveAsync();
                _logger.LogInformation($"User: {UserId} deleted his reply with id: {id}");
                return Ok(reply.ToVModel());
            }
            catch (Exception e)
            {
                _logger.LogError($"reply deletion by {UserId} failed due to {e.Message ?? e.InnerException.Message}");
                return BadRequest(e.ToResponse(500));
            }
        }

        #region Helpers
        private int _commentId
        {
            get => Convert.ToInt32(RouteData.Values["commentId"]);
        }

        protected Expression<Func<Reply, bool>> filter
        {
            get => p => p.CommentId == _commentId;
        }

        #endregion

    }
}
