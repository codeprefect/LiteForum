using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.SwaggerGen;
using CP.Repositories.Interfaces;
using CP.Repositories;
using LiteForum.Services.Interfaces;
using LiteForum.Services;
using LiteForum.Entities.Models;
using LiteForum.Models;
using LiteForum.Helpers;
using LiteForum_UI;


namespace LiteForum
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

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
            }).AddNewtonsoftJson();

            services.AddApiVersioning(options => {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
            });

            services.AddVersionedApiExplorer(options =>
            {
                // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                // note: the specified format code will format the version as "'v'major[.minor][-status]"
                options.GroupNameFormat = "'v'VVV";

                // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                // can also be used to control the format of the API version in route templates
                options.SubstituteApiVersionInUrl = true;
            });

            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            services.AddSwaggerGen(options =>
            {
                options.OperationFilter<SwaggerDefaultValues>();
                options.IncludeXmlComments(XmlCommentsFilePath);
            });

            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            IApiVersionDescriptionProvider apiVersionProvider)
        {
            app.UseResponseCompression();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBlazorDebugging();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(name: "default", template: "{controller}/{action}/{id?}");
            });

            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                options.RoutePrefix = "docs";
                // build a swagger endpoint for each discovered API version
                foreach ( var description in apiVersionProvider.ApiVersionDescriptions )
                {
                    options.SwaggerEndpoint(
                        $"/swagger/{description.GroupName}/swagger.json",
                        description.GroupName.ToUpperInvariant());
                }
            });

            app.UseBlazor<LiteForum_UI.Startup>();

            app.UseAuthentication();
            app.UseAuthorization();

            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetService<LiteForumDbContext>().Database.Migrate();
                StartupHelper.CreateRolesAndAdminUser(serviceScope.ServiceProvider, Configuration).Wait();
                if (env.IsTest()) {
                    serviceScope.ServiceProvider.GetService<LiteForumDbContext>().EnsureSeeded(Configuration).Wait();
                }
            }
        }

        static string XmlCommentsFilePath
        {
            get
            {
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var fileName = typeof( Startup ).GetTypeInfo().Assembly.GetName().Name + ".xml";
                return Path.Combine( basePath, fileName );
            }
        }
    }
}
