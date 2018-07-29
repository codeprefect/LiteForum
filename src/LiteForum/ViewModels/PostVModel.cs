using System;
using System.Collections.Generic;

namespace LiteForum.ViewModels {
    public class PostVModel : BaseVModel {
        public string Title { get; set; }
        public ICollection<CommentVModel> Comments { get; set; }
        public int CommentsCount { get; set; }
        public DateTime? LastCommentAt { get; set; }
        public string LastCommentBy { get; set; }
        public CategoryVModel Category { get; internal set; }
        public int CategoryId { get; set; }
    }
}