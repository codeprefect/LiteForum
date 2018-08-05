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
    [Authorize(policy: "Authenticated")]
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : BaseApiController
    {
        private readonly ILogger<PostController> _logger;
        private readonly IDataService<LiteForumDbContext, Post> _posts;

        public PostController(ILogger<PostController> logger,
            IDataService<LiteForumDbContext, Post> posts)
        {
            _logger = logger;
            _posts = posts;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            var posts = await _posts.GetAsync(includeProperties: "Category,Comments,Comments.User,Comments.Replies");
            return Ok(posts.Select(p => p.ToVModel()));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSingle(int id, bool withChild)
        {
            Expression<Func<Post, bool>> filter = p => p.Id == id;
            var post = withChild ?
                await _posts.GetOneAsync(filter: filter, includeProperties: "Category,Comments,Comments.User,Comments.Replies") :
                await _posts.GetOneAsync(filter: filter);
            if (post == null) return NotFound();
            return Ok(post.ToVModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PostVModel post)
        {
            var newPost = post.ToModel();
            newPost.UserId = UserId;
            newPost = _posts.Create(newPost, UserId);
            await _posts.SaveAsync();
            _logger.LogInformation($"User: {UserId} created a new post {newPost}");
            return Created(Request.Path.Value, newPost.ToVModel());
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] PostVModel post)
        {
            var oldPost = await _posts.GetByIdAsync(post.Id);
            if (oldPost.UserId != UserId) throw new UnauthorizedAccessException(userMismatchMessage);
            oldPost.Title = post.Title;
            _posts.Update(oldPost, UserId);
            await _posts.SaveAsync();
            _logger.LogInformation($"User: {UserId} modified a his post {oldPost}");
            return Ok(oldPost.ToVModel());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _posts.GetByIdAsync(id);
            if (post.UserId != UserId && !IsAdmin) throw new UnauthorizedAccessException(userMismatchMessage);
            _posts.Delete(id);
            await _posts.SaveAsync();
            _logger.LogInformation($"User: {UserId} deleted his post with id: {id}");
            return Ok(new LiteForumResponseMessage(200, "deleted successfully"));
        }

        #region Helpers
        private const string userMismatchMessage = "post was authored by another user.";
        #endregion
    }
}
