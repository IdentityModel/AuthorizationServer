using Microsoft.Owin.Security.OAuth;
using System.Threading.Tasks;

namespace Thinktecture.IdentityModel.Owin
{
    public class HeaderOAuthBearerProvider : OAuthBearerAuthenticationProvider
    {
        readonly string _name;

        public HeaderOAuthBearerProvider(string name)
        {
            _name = name;
        }

         public override async Task RequestToken(OAuthRequestTokenContext context)
        {
            var value = context.Request.Headers.Get(_name);

            if (!string.IsNullOrEmpty(value))
            {
                context.Token = value;
            }
        }
    }
}