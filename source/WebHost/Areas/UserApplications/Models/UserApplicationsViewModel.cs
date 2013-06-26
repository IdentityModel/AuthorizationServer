using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

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