namespace Thinktecture.IdentityModel.Owin
{
    public static class TokenLocation
    {
        public static QueryStringOAuthBearerProvider QueryString(string name)
        {
            return new QueryStringOAuthBearerProvider(name);
        }

        public static HeaderOAuthBearerProvider Header(string name)
        {
            return new HeaderOAuthBearerProvider(name);
        }
    }
}