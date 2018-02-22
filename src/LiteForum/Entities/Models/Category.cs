using System.Collections.Generic;

namespace LiteForum.Entities.Models
{
    public class Category : Entity<int>
    {
        public string Name { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
    }
}
