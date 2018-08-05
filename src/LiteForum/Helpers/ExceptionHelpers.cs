
using System;
using System.Linq;
using LiteForum.Entities.Models;
using LiteForum.Models;
using LiteForum.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace LiteForum.Helpers
{
    public static class ExceptionHelpers
    {
        public static LiteForumResponseMessage ToResponse(this Exception e, int statusCode)
        {
            return new LiteForumResponseMessage(statusCode, e.Message ?? e.InnerException.Message);
        }
    }

    public class LiteForumExceptionFilter : ExceptionFilterAttribute
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ILogger<Exception> _logger;

        public LiteForumExceptionFilter(IHostingEnvironment hostingEnvironment, ILogger<Exception> logger)
        {
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            if (!context.HttpContext.Request.Path.StartsWithSegments("/api"))
            {
                return;
            }

            var result = new BadRequestObjectResult(context.Exception.ToResponse(400));
            _logger.LogError($"an error occurred: {context.Exception.Message}");
            context.Result = result;
        }
    }
}
