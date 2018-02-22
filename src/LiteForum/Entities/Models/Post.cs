using System;
using System.Collections.Generic;

namespace LiteForum.Entities.Models
{
    public class Post : HasUser<int>
    {
        public string Title { get; set; }
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}
