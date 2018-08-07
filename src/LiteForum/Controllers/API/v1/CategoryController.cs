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

namespace LiteForum.Controllers.API.v1
{
    [Route("api/v{api-version:apiVersion}/[controller]")]
    public class CategoryController : BaseV1ApiController
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
            var categories = await _categories.GetAsync();
            return Ok(categories.Select(c => c.ToVModel()));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSingle(int id, bool withChild)
        {
            Expression<Func<Category, bool>> filter = p => p.Id == id;
            var category = withChild ?
                await _categories.GetOneAsync(filter: filter, includeProperties: "Posts") :
                await _categories.GetOneAsync(filter: filter);
            return Ok(category.ToVModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CategoryVModel category)
        {
            var newCategory = category.ToModel();
            _categories.Create(newCategory, UserId);
            await _categories.SaveAsync();
            _logger.LogInformation($"User: {UserId} created a new category {newCategory}");
            return Created(Request.Path.Value, newCategory.ToVModel());
        }

        [HttpPut]
        [Authorize(policy: AppConstants.String.Roles.Admin)]
        public async Task<IActionResult> Update([FromBody]CategoryVModel category)
        {
            var oldCategory = await _categories.GetByIdAsync(category.Id);
            oldCategory.Name = category.Name;
            _categories.Update(oldCategory, UserId);
            await _categories.SaveAsync();
            _logger.LogInformation($"User: {UserId} modified a his category {oldCategory}");
            return Ok(oldCategory.ToVModel());
        }

        [HttpDelete("{id}")]
        [Authorize(policy: AppConstants.String.Roles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categories.GetByIdAsync(id);
            _categories.Delete(id);
            await _categories.SaveAsync();
            _logger.LogInformation($"User: {UserId} deleted his category with id: {id}");
            return Ok(new LiteForumResponseMessage(200, "deleted successfully"));
        }
    }
}
