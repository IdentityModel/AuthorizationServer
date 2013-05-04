using System.Collections.Generic;

namespace Thinktecture.AuthorizationServer.Core.Models
{
    public class ValidatedAuthorizeRequest
    {
        public Application Application { get; set; }
        public Client Client { get; set; }
        public RedirectUri RedirectUri { get; set; }
        public string ResponseType { get; set; }
        public List<Scope> Scopes { get; set; }
    }
}
