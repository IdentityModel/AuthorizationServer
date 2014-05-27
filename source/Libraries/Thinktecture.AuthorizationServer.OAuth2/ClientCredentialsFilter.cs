using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Http.Results;

namespace Thinktecture.AuthorizationServer.OAuth2
{
    public class ClientCredentialsFilter : Attribute, IAuthenticationFilter
    {
        public bool AllowMultiple { get { return false; } }

        public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            string id, secret;
            var isMalformed = true;
            
			if (TryParseBasicAuthenticationScheme(context.Request, out id, out secret, out isMalformed) 
				|| TryParseQueryStringAuthenticationScheme(context.Request, out id, out secret, ref isMalformed))
            {
                var identity = new ClaimsIdentity("Basic");
                identity.AddClaim(new Claim("client_id", id));
                identity.AddClaim(new Claim("secret", secret));

                context.Principal = new ClaimsPrincipal(identity);
            }

	        if (isMalformed)
            {
                context.ErrorResult = new BadRequestResult(context.Request);
            }

            return Task.FromResult<object>(null);
        }

	    public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult<object>(null);
        }

        private bool TryParseBasicAuthenticationScheme(HttpRequestMessage request, out string id, out string secret, out bool isMalformed)
        {
            isMalformed = true;
            id = ""; secret = "";

            if (request.Headers.Authorization == null ||
                !request.Headers.Authorization.Scheme.Equals("Basic", StringComparison.OrdinalIgnoreCase))
            {
                isMalformed = false;
                return false;
            }

            string pair;
            try
            {
                pair = Encoding.UTF8.GetString(
                    Convert.FromBase64String(request.Headers.Authorization.Parameter));
            }
            catch (FormatException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }

            var ix = pair.IndexOf(':');
            if (ix == -1) return false;
            
            id = pair.Substring(0, ix);
            secret = pair.Substring(ix + 1);

            isMalformed = false;
            return true;
        }

		private bool TryParseQueryStringAuthenticationScheme(HttpRequestMessage request, out string id, out string secret, ref bool isMalformed)
		{
			id = ""; secret = "";

			var content = request.Content.ReadAsStringAsync().Result;

			var coll = HttpUtility.ParseQueryString(content);

			if (coll["client_id"] == null ||
				coll["client_secret"] == null)
			{
				return false;
			}

			id = coll["client_id"];
			secret = coll["client_secret"];

			isMalformed = false;
			return true;
		}
    }
}
