using System;
using System.Collections.Generic;

namespace LiteForum.Entities.Models
{
    public class Post : HasUser<int>
    {
        public string Content { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}
