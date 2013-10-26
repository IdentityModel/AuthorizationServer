using Nancy;
using Nancy.Bootstrapper;
using Nancy.Security;
using Nancy.TinyIoc;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace ResourceServer.Security
{
    public class SecurityBootstrapper : DefaultNancyBootstrapper
    {
        protected override void RequestStartup(TinyIoCContainer requestContainer, IPipelines pipelines, NancyContext context)
        {
            pipelines.BeforeRequest += CheckOwinContext;

            base.RequestStartup(requestContainer, pipelines, context);
        }

        Task<Response> CheckOwinContext(NancyContext context, CancellationToken token)
        {
            var principal = context.GetOwinPrincipal();
            
            if (principal.Identity.IsAuthenticated)
            {
                context.CurrentUser = new ClaimsUserIdentity(principal);
            }

            return Task.FromResult<Response>(null);
        }
    }
}