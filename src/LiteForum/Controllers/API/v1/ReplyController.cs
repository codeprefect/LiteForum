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

namespace LiteForum.Controllers.API.v1
{
    [Route("api/v{api-version:apiVersion}/comment/{commentId}/[controller]")]
    public class ReplyController : BaseV1ApiController
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
        public async Task<IActionResult> Get(int commentId)
        {
            var replies = await _replies.GetAsync(filter: r => r.CommentId == commentId);
            return Ok(replies.Select(c => c.ToVModel()));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSingle(int commentId, int id)
        {
            var reply = await _replies.GetOneAsync(filter: r => r.Id == id && r.CommentId == commentId);
            if (reply == null) return NotFound();
            return Ok(reply.ToVModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(int commentId, [FromBody] ReplyVModel reply)
        {
            var newReply = reply.ToModel();
            newReply.CommentId = commentId;
            newReply.UserId = UserId;
            newReply = _replies.Create(newReply, UserId);
            await _replies.SaveAsync();
            _logger.LogInformation($"User: {UserId} created a new reply {newReply}");
            return Created(Request.Path.Value, newReply.ToVModel());
        }

        [HttpPut]
        public async Task<IActionResult> Update(int commentId, [FromBody] ReplyVModel reply)
        {
            var oldReply = await _replies.GetByIdAsync(reply.Id);
            if (oldReply.UserId != UserId) throw new UnauthorizedAccessException(userMismatchMessage);
            if (oldReply.CommentId != commentId) throw new AccessViolationException(resourceMismatchMessage);

            oldReply.Content = reply.Content;
            oldReply = _replies.Update(oldReply, UserId);
            await _replies.SaveAsync();
            _logger.LogInformation($"User: {UserId} modified a his reply {oldReply}");
            return Ok(oldReply.ToVModel());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int commentId, int id)
        {
            var reply = await _replies.GetByIdAsync(id);
            if (reply.UserId != UserId) throw new UnauthorizedAccessException(userMismatchMessage);
            if (reply.CommentId != commentId) throw new Exception(resourceMismatchMessage);
            _replies.Delete(id);
            await _replies.SaveAsync();
            _logger.LogInformation($"User: {UserId} deleted his reply with id: {id}");
            return Ok(reply.ToVModel());
        }
    }
}
