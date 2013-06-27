using System.Security.Claims;

namespace Thinktecture.AuthorizationServer.WebHost.Areas.UserApplications.Models
{
    public class UserApplicationsViewModel
    {
        public string Subject
        {
            get
            {
                return ClaimsPrincipal.Current.GetSubject();
            }
        }


    }
}