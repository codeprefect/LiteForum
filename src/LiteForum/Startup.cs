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
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.IO;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.ResponseCompression;
using System.Net.Mime;
using Microsoft.AspNetCore.Blazor.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.PlatformAbstractions;
using System.Reflection;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.Extensions.Options;

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
            }).AddJsonOptions(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

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

            services.AddResponseCompression(options =>
            {
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
                {
                    MediaTypeNames.Application.Octet,
                    WasmMediaTypeNames.Application.Wasm,
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            IApiVersionDescriptionProvider apiVersionProvider)
        {
            app.UseResponseCompression();

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

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
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
