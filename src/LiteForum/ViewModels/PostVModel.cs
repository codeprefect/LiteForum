using System;
using System.Collections.Generic;

namespace LiteForum.ViewModels
{
    public class PostVModel : BaseVModel
    {
        public string Title { get; set; }
        public ICollection<CommentVModel> Comments { get; set; }
        public int CommentsCount { get; set; }
        public DateTime? LastCommentAt { get; set; }
        public string LastCommentBy { get; set; }
        public string Category { get; internal set; }
    }
}