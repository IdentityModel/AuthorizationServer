using Microsoft.Owin.Security.OAuth;
using System.Threading.Tasks;

namespace Thinktecture.IdentityModel.Owin
{
    public class QueryStringOAuthBearerProvider : OAuthBearerAuthenticationProvider
    {
        readonly string _name;

        public QueryStringOAuthBearerProvider(string name)
        {
            _name = name;
        }

        public override async Task RequestToken(OAuthRequestTokenContext context)
        {
            var value = context.Request.Query[_name];

            if (!string.IsNullOrEmpty(value))
            {
                context.Token = value;
            }
        }
    }
}