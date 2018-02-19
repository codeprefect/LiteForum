using System;
using System.Collections.Generic;

namespace LiteForum.ViewModels
{
    public class CommentVModel : BaseVModel
    {
        public string Content { get; set; }
        public int PostId { get; set; }
        public ICollection<ReplyVModel> Replies { get; set; }
        public int RepliesCount { get; set; }
        public DateTime? LastReplyAt { get; set; }
        public string LastReplyBy { get; set; }
    }
}
