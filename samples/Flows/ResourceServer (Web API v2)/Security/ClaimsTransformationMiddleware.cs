using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Thinktecture.IdentityModel.Owin
{
    public class ClaimsTransformationMiddleware
    {
        readonly ClaimsTransformationOptions _options;
        readonly Func<IDictionary<string, object>, Task> _next;

        public ClaimsTransformationMiddleware(Func<IDictionary<string, object>, Task> next, ClaimsTransformationOptions options)
        {
            _next = next;
            _options = options;
        }

        public async Task Invoke(IDictionary<string, object> env)
        {
            // use Katana OWIN abstractions (optional)
            var context = new OwinContext(env);
            var transformer = _options.ClaimsAuthenticationManager;
            
            if (context.Authentication != null && 
                context.Authentication.User != null)
            {
                context.Authentication.User = transformer.Authenticate(
                    context.Request.Uri.AbsoluteUri,
                    context.Authentication.User);
            }

            await _next(env);
        }
    }
}