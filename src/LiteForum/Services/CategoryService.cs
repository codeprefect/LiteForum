using CP.Repositories.Interfaces;
using LiteForum.Entities.Models;
using LiteForum.Models;

namespace LiteForum.Services
{
    public class CategoryService : DataService<LiteForumDbContext, Category>
    {
        public CategoryService(IRepository<LiteForumDbContext> repository) : base(repository)
        { }
    }
}
