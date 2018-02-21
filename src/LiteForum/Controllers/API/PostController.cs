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
    [Route("api/[controller]")]
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
            try
            {
                var posts = await _posts.GetAsync();
                return Ok(posts.Select(p => p.ToVModel()));
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to fetch posts due to {e.Message ?? e.InnerException.Message}");
                return BadRequest(e.ToResponse(500));
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSingle(int id, bool withChild)
        {
            if(id <= 0) return BadRequest($"submitted id: {id} is not valid");

            Expression<Func<Post, bool>> filter = p => p.Id == id;
            try
            {
                var post = withChild ?
                    await _posts.GetOneAsync(filter: filter, includeProperties: "Comments") :
                    await _posts.GetOneAsync(filter: filter);
                return Ok(post.ToVModel());
            }
            catch (Exception e)
            {
                _logger.LogError($"failed to get post with id: {id}, due to {e.Message ?? e.InnerException.Message}");
                return BadRequest(e.ToResponse(500));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]PostVModel post)
        {
            if(!ModelState.IsValid) return BadRequest($"submitted post has {ModelState.ErrorCount} errors");

            try
            {
                var newPost = post.ToModel();
                newPost.UserId = UserId;
                _posts.Create(newPost, UserId);
                await _posts.SaveAsync();
                _logger.LogInformation($"User: {UserId} created a new post {newPost}");
                return Created(Request.Path.Value, newPost.ToVModel());
            }
            catch(Exception e)
            {
                _logger.LogError($"post creation by {UserId} failed due to {e.Message ?? e.InnerException.Message}");
                return BadRequest(e.ToResponse(500));
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody]PostVModel post)
        {
            if (!ModelState.IsValid) return BadRequest($"submitted post has {ModelState.ErrorCount} errors");

            try
            {
                var oldPost = await _posts.GetByIdAsync(post.Id);
                if(oldPost.UserId != UserId)
                {
                    throw new Exception("you do not have write access to this post");
                }
                oldPost.Content = post.Content;
                _posts.Update(oldPost, UserId);
                await _posts.SaveAsync();
                _logger.LogInformation($"User: {UserId} modified a his post {oldPost}");
                return Ok(oldPost.ToVModel());
            }
            catch (Exception e)
            {
                _logger.LogError($"post modification by {UserId} failed due to {e.Message ?? e.InnerException.Message}");
                return BadRequest(e.ToResponse(500));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if(id <= 0) return BadRequest(new LiteForumResponseMessage(500, $"submitted id: {id} is invalid"));

            try
            {
                var post = await _posts.GetByIdAsync(id);
                if(post.UserId != UserId) throw new Exception ("sorry, you cannot delete this post.");
                _posts.Delete(id);
                await _posts.SaveAsync();
                _logger.LogInformation($"User: {UserId} deleted his post with id: {id}");
                return Ok(new LiteForumResponseMessage(200, "deleted successfully"));
            }
            catch (Exception e)
            {
                _logger.LogError($"post deletion by {UserId} failed due to {e.Message ?? e.InnerException.Message}");
                return BadRequest(e.ToResponse(500));
            }
        }
    }
}
