using System.Linq;
using LiteForum.Entities.Models;
using LiteForum.ViewModels;

namespace LiteForum.Helpers
{
    public static class Mappings
    {
        public static PostVModel ToVModel(this Post p)
        {
            return new PostVModel
            {
                Id = p.Id,
                Title = p.Title,
                ModifiedDate = p.ModifiedDate,
                CreatedDate = p.CreatedDate,
                User = p.User?.UserName,
                Comments = p.Comments?.Select(c => c.ToVModel()).ToList(),
                CommentsCount = p.Comments.Count()
            };
        }

        public static Post ToModel(this PostVModel p)
        {
            return new Post
            {
                Id = p.Id,
                Title = p.Title,
            };
        }

        public static CommentVModel ToVModel(this Comment c)
        {
            return new CommentVModel
            {
                Id = c.Id,
                Content = c.Content,
                ModifiedDate = c.ModifiedDate,
                CreatedDate = c.CreatedDate,
                PostId = c.PostId,
                User = c.User?.UserName,
                Replies = c.Replies?.Select(r => r.ToVModel()).ToList(),
                RepliesCount = c.Replies.Count()
            };
        }

        public static Comment ToModel(this CommentVModel c)
        {
            return new Comment
            {
                Id = c.Id,
                Content = c.Content
            };
        }

        public static ReplyVModel ToVModel(this Reply r)
        {
            return new ReplyVModel
            {
                Id = r.Id,
                Content = r.Content,
                ModifiedDate = r.ModifiedDate,
                CreatedDate = r.CreatedDate,
                User = r.User?.UserName,
                CommentId = r.CommentId,
            };
        }

        public static Reply ToModel(this ReplyVModel r)
        {
            return new Reply
            {
                Id = r.Id,
                Content = r.Content
            };
        }

        public static CategoryVModel ToVModel(this Category c)
        {
            return new CategoryVModel
            {
                Id = c.Id,
                Name = c.Name,
                ModifiedDate = c.ModifiedDate,
                CreatedDate = c.CreatedDate
            };
        }

        public static Category ToModel(this CategoryVModel c)
        {
            return new Category
            {
                Id = c.Id,
                Name = c.Name
            };
        }
    }
}
