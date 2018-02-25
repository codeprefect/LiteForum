using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteForum.Entities.Models;
using LiteForum.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;

namespace LiteForum.Helpers
{
    public static class DbContextExtension
    {
        public static bool AllMigrationsApplied(this DbContext context)
        {
            var applied = context.GetService<IHistoryRepository>()
                .GetAppliedMigrations()
                .Select(m => m.MigrationId);

            var total = context.GetService<IMigrationsAssembly>()
                .Migrations
                .Select(m => m.Key);

            return !total.Except(applied).Any();
        }

        public async static Task EnsureSeeded(this LiteForumDbContext context, IConfiguration configuration)
        {
            if (!context.Replies.Any())
            {
                var userName = configuration.GetSection("AppSettings")["AdminUsername"];
                var user = await context.Users.FirstOrDefaultAsync(u => u.UserName == userName);

                if (user != null)
                {
                    // add categories
                    if (!context.Categories.Any())
                    {
                        var categories = new List<Category> {
                            new Category {
                                Name = "Programming",
                                CreatedBy = user.Id,
                                CreatedDate = DateTime.UtcNow
                            },
                            new Category {
                                Name = "Mathematics",
                                CreatedBy = user.Id,
                                CreatedDate = DateTime.UtcNow
                            }
                        };
                        context.Categories.AddRange(categories);
                        await context.SaveChangesAsync();
                    }

                    // add posts
                    if (!context.Posts.Any())
                    {
                        var posts = new List<Post> {
                            new Post {
                                Title = "Why do we have scarcity of quality programmers if programming is indeed very easy to learn",
                                CategoryId = context.Categories.FirstOrDefault().Id,
                                UserId = user.Id,
                                CreatedBy = user.Id,
                                CreatedDate = DateTime.UtcNow,
                                Comments = new List<Comment> {

                                }
                            },
                            new Post {
                                Title = "If you are given the opportunity to choose either owning facebook or becoming the president of China, which would you choose?",
                                CategoryId = context.Categories.LastOrDefault().Id,
                                UserId = user.Id,
                                CreatedBy = user.Id,
                                CreatedDate = DateTime.UtcNow,
                            }
                        };
                        context.Posts.AddRange(posts);
                        await context.SaveChangesAsync();
                    }

                    if (!context.Comments.Any())
                    {   // create comments dummy
                        var comments = new List<Comment>
                        {
                            new Comment {
                                Content = "Programming is not hard, but seeing teachers who can teach clean code is hard",
                                UserId = user.Id,
                                CreatedBy = user.UserName,
                                CreatedDate = DateTime.UtcNow,
                                PostId = context.Posts.FirstOrDefault().Id
                            },
                            new Comment {
                                Content = "Prepare, and the best of preparation is the fear of Allah",
                                UserId = user.Id,
                                CreatedBy = user.UserName,
                                CreatedDate = DateTime.UtcNow,
                                PostId = context.Posts.LastOrDefault().Id
                            }
                        };
                        
                        context.Comments.AddRange(comments);
                        await context.SaveChangesAsync();
                    }

                    // create replies seed
                    var replies = new List<Reply>
                    {   new Reply {
                            Content = "When you see someone who strongly believes he can make it decide repeated failures, he is an Arsenal fan",
                            UserId = user.Id,
                            CreatedBy = user.UserName,
                            CreatedDate = DateTime.UtcNow,
                            CommentId = context.Comments.FirstOrDefault().Id
                        },
                        new Reply {
                            Content = "I chose to support Barcelona FC because when I was growing up, I believe winning is their default and loosing is a mistake",
                            UserId = user.Id,
                            CreatedBy = user.UserName,
                            CreatedDate = DateTime.UtcNow,
                            CommentId = context.Comments.LastOrDefault().Id
                        }
                    };

                    context.Replies.AddRange(replies);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
