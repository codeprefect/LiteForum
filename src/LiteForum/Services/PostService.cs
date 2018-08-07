using CP.Repositories.Interfaces;
using LiteForum.Entities.Models;
using LiteForum.Models;

namespace LiteForum.Services
{
    public class PostService : DataWithUserService<LiteForumDbContext, Post>
    {
        public PostService(IRepository<LiteForumDbContext> posts) : base(posts) { }
    }
}
