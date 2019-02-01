using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CP.Repositories.Interfaces;
using CP.Repositories;
using LiteForum.Entities.Models;
using LiteForum.Helpers;
using LiteForum.Models;
using LiteForum.Services;
using LiteForum.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;
using System.Reflection;
using System.IO;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using SwashbuckleAspNetVersioningShim;

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
                options.DefaultAuthenticateScheme = AppConstants.String.AuthSchemes.Identity;
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
                options.AddPolicy(AppConstants.String.Policies.Authenticated, policy =>
                {
                    policy.AddAuthenticationSchemes(
                        AppConstants.String.AuthSchemes.Identity,
                        AppConstants.String.AuthSchemes.JwtBearer)
                        .RequireAuthenticatedUser()
                        .Build();
                });

                options.AddPolicy(AppConstants.String.Policies.Admin, policy =>
                {
                    policy.RequireRole(AppConstants.String.Roles.Admin)
                        .RequireAuthenticatedUser()
                        .Build();
                });
            });

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IDataService<LiteForumDbContext, Post>, PostService>();
            services.AddScoped<IDataService<LiteForumDbContext, Comment>, CommentService>();
            services.AddScoped<IDataService<LiteForumDbContext, Reply>, ReplyService>();
            services.AddScoped<IDataService<LiteForumDbContext, Category>, CategoryService>();

            services.AddMvc(config =>
            {
                config.Filters.Add(typeof(LiteForumExceptionFilter));
                //config.UseCentralRoutePrefix(new RouteAttribute("api/v{version}"));
            }).AddJsonOptions(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            services.AddMvcCore().AddVersionedApiExplorer();
            services.AddApiVersioning();//options => options.ReportApiVersions = true);
            services.AddSwaggerGen(options =>
            {
                var provider = services.BuildServiceProvider()
                            .GetRequiredService<IApiVersionDescriptionProvider>();
                options.ConfigureSwaggerVersions(provider, "LiteForum API Documentation v{0}");
            });

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "LiteForum_UI/dist";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            IApiVersionDescriptionProvider apiVersionProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.ConfigureSwaggerVersions(apiVersionProvider, new SwaggerVersionOptions
                {
                    DescriptionTemplate = "Version {0} docs",
                    RouteTemplate = "/swagger/{0}/swagger.json",
                });
                c.RoutePrefix = "docs";
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "../LiteForum_UI";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });

            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetService<LiteForumDbContext>().Database.Migrate();
                StartupHelper.CreateRolesAndAdminUser(serviceScope.ServiceProvider, Configuration).Wait();
                if (env.IsTest()) {
                    serviceScope.ServiceProvider.GetService<LiteForumDbContext>().EnsureSeeded(Configuration).Wait();
                }
            }
        }
    }
}
