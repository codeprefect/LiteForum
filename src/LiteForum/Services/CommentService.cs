using LiteForum.Data.Interfaces;
using LiteForum.Entities.Models;
using LiteForum.Models;

namespace LiteForum.Services
{
    public class CommentService : DataService<LiteForumDbContext, Comment>
    {
        public CommentService(IRepository<LiteForumDbContext> comments) : base(comments)
        { }
    }
}
