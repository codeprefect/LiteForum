using LiteForum.Data.Interfaces;
using LiteForum.Entities.Models;
using LiteForum.Models;

namespace LiteForum.Services
{
    public class PostService : DataService<LiteForumDbContext, Post>
    {
        public PostService(IRepository<LiteForumDbContext> posts) : base(posts)
        { }
    }
}
