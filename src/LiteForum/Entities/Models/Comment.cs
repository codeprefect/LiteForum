using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace LiteForum.Entities.Models
{
    public class Comment : HasUser<int>
    {
        public string Content { get; set; }
        public int PostId { get; set; }
        public virtual Post Post { get; set; }
        public ICollection<Reply> Replies { get; set; }
    }
}
