using LiteForum.Data.Interfaces;
using LiteForum.Entities.Models;
using LiteForum.Models;

namespace LiteForum.Services
{
    public class ReplyService : DataService<LiteForumDbContext, Reply>
    {
        public ReplyService(IRepository<LiteForumDbContext> replies) : base(replies)
        { }
    }
}
