namespace LiteForum.Helpers
{
    public static class AppConstants
    {
        public static class String
        {
            public static class Roles
            {
                public const string Admin = "admin";
                public const string Member = "member";
            }

            public static class Policies
            {
                public const string Authenticated = "authed";
                public const string Admin = "admin";
            }

            public static class AuthSchemes
            {
                public const string JwtBearer = "JwtBearer";
                public const string Identity = "Identity.Application";
            }
        }
    }
}
