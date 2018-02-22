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
    public class CategoryController : BaseApiController
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly IDataService<LiteForumDbContext, Category> _categories;

        public CategoryController(ILogger<CategoryController> logger,
            IDataService<LiteForumDbContext, Category> categories)
        {
            _logger = logger;
            _categories = categories;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            try
            {
                var categories = await _categories.GetAsync();
                return Ok(categories.Select(c => c.ToVModel()));
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to fetch categories due to {e.Message ?? e.InnerException.Message}");
                return BadRequest(e.ToResponse(500));
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSingle(int id, bool withChild)
        {
            if(id <= 0) return BadRequest($"submitted id: {id} is not valid");

            Expression<Func<Category, bool>> filter = p => p.Id == id;
            try
            {
                var category = withChild ?
                    await _categories.GetOneAsync(filter: filter, includeProperties: "Comments") :
                    await _categories.GetOneAsync(filter: filter);
                return Ok(category.ToVModel());
            }
            catch (Exception e)
            {
                _logger.LogError($"failed to get category with id: {id}, due to {e.Message ?? e.InnerException.Message}");
                return BadRequest(e.ToResponse(500));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CategoryVModel category)
        {
            if(!ModelState.IsValid) return BadRequest($"submitted category has {ModelState.ErrorCount} errors");

            try
            {
                var newCategory = category.ToModel();
                _categories.Create(newCategory, UserId);
                await _categories.SaveAsync();
                _logger.LogInformation($"User: {UserId} created a new category {newCategory}");
                return Created(Request.Path.Value, newCategory.ToVModel());
            }
            catch(Exception e)
            {
                _logger.LogError($"category creation by {UserId} failed due to {e.Message ?? e.InnerException.Message}");
                return BadRequest(e.ToResponse(500));
            }
        }

        [HttpPut]
        [Authorize(policy: AppConstants.Roles.Admin)]
        public async Task<IActionResult> Update([FromBody]CategoryVModel category)
        {
            if (!ModelState.IsValid) return BadRequest($"submitted category has {ModelState.ErrorCount} errors");

            try
            {
                var oldCategory = await _categories.GetByIdAsync(category.Id);
                oldCategory.Name = category.Name;
                _categories.Update(oldCategory, UserId);
                await _categories.SaveAsync();
                _logger.LogInformation($"User: {UserId} modified a his category {oldCategory}");
                return Ok(oldCategory.ToVModel());
            }
            catch (Exception e)
            {
                _logger.LogError($"category modification by {UserId} failed due to {e.Message ?? e.InnerException.Message}");
                return BadRequest(e.ToResponse(500));
            }
        }

        [HttpDelete("{id}")]
        [Authorize(policy: AppConstants.Roles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            if(id <= 0) return BadRequest(new LiteForumResponseMessage(500, $"submitted id: {id} is invalid"));

            try
            {
                var category = await _categories.GetByIdAsync(id);
                _categories.Delete(id);
                await _categories.SaveAsync();
                _logger.LogInformation($"User: {UserId} deleted his category with id: {id}");
                return Ok(new LiteForumResponseMessage(200, "deleted successfully"));
            }
            catch (Exception e)
            {
                _logger.LogError($"category deletion by {UserId} failed due to {e.Message ?? e.InnerException.Message}");
                return BadRequest(e.ToResponse(500));
            }
        }
    }
}
