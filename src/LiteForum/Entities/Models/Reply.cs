using System;

namespace LiteForum.Entities.Models
{
    public class Reply : HasUser<int>
    {
        public string Content { get; set; }
        public int CommentId { get; set; }
        public virtual Comment Comment { get; set; }
    }
}
