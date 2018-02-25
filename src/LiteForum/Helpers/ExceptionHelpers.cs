
using System;
using System.Linq;
using LiteForum.Entities.Models;
using LiteForum.Models;
using LiteForum.ViewModels;

namespace LiteForum.Helpers
{
    public static class ExceptionHelpers
    {
        public static LiteForumResponseMessage ToResponse(this Exception e, int statusCode)
        {
            return new LiteForumResponseMessage(statusCode, e.Message ?? e.InnerException.Message);
        }
    }
}
