using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LiteForum.Data.Interfaces;
using LiteForum.Data.Repositories;
using LiteForum.Entities.Models;
using LiteForum.Helpers;
using LiteForum.Models;
using LiteForum.Services;
using LiteForum.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LiteForum
{
    public class Startup
    {
        private readonly IConfiguration Configuration;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<LiteForumDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddIdentity<LiteForumUser, IdentityRole>()
                .AddEntityFrameworkStores<LiteForumDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Identity.Application";
            }).ConfigureJwtAuth(Configuration);

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "AuthCookie";
                options.Cookie.Path = "/";
                options.Cookie.HttpOnly = false;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.SlidingExpiration = true;
                options.Events.OnRedirectToLogin = async ctx =>
                {
                    if (ctx.Request.Path.StartsWithSegments("/api") && ctx.Response.StatusCode == 200)
                    {
                        ctx.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    }
                    else
                    {
                        ctx.Response.Redirect(ctx.RedirectUri);
                    }

                    await Task.Yield();
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Authenticated", policy =>
                {
                    policy.AddAuthenticationSchemes("Identity.Application", "JwtBearer")
                        .RequireAuthenticatedUser()
                        .Build();
                });
            });

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IDataService<LiteForumDbContext, Post>, PostService>();
            services.AddScoped<IDataService<LiteForumDbContext, Comment>, CommentService>();
            services.AddScoped<IDataService<LiteForumDbContext, Reply>, ReplyService>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
