using Microsoft.Owin;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Thinktecture.IdentityModel.Owin
{
    public class KatanaClaimsTransformationMiddleware : OwinMiddleware
    {
        ClaimsAuthenticationManager _claimsAuthenticationManager;

        public KatanaClaimsTransformationMiddleware(OwinMiddleware next, ClaimsAuthenticationManager claimsAuthenticationManager) : base(next)
        {
            if (claimsAuthenticationManager == null)
            {
                throw new ArgumentNullException("claimsAuthenticationManager");
            }

            _claimsAuthenticationManager = claimsAuthenticationManager;
        }

        public override Task Invoke(IOwinContext context)
        {
            if (context.Authentication.User != null)
            {
                context.Authentication.User = _claimsAuthenticationManager.Authenticate(
                    context.Request.Uri.AbsoluteUri, 
                    context.Authentication.User);
            }

            return Next.Invoke(context);
        }
    }
}