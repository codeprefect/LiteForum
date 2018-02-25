using LiteForum.Entities.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LiteForum.Models
{
    public class LiteForumDbContext : IdentityDbContext<LiteForumUser>
    {
        public LiteForumDbContext(DbContextOptions<LiteForumDbContext> options) : base(options)
        {

        }

        public LiteForumDbContext() { }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Reply> Replies { get; set; }
    }
}
