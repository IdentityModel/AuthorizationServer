using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Thinktecture.AuthorizationServer.OAuth2
{
    public class CustomNameRegistry : IssuerNameRegistry
    {
        Dictionary<string, string> _allowedIssuers = new Dictionary<string, string>();

        public void AddTrustedIssuer(string certificateThumbprint, string name) {
            _allowedIssuers.Add(certificateThumbprint, name);
        }


        public override string GetIssuerName(SecurityToken securityToken)
        {
            if (!(securityToken is X509SecurityToken)) throw new SecurityTokenValidationException("Invalid token.");
            X509Certificate2 x509Token = (securityToken as X509SecurityToken).Certificate as X509Certificate2;
            // in the X509 case, the X509 token has no notion of issuer name
            string result="";
            
            bool issuerTokenValid = _allowedIssuers.TryGetValue(x509Token.Thumbprint, out result);
            if (!issuerTokenValid || result == "" || x509Token.NotAfter < System.DateTime.Now) throw new SecurityTokenValidationException("Untrusted issuer token.");
            
            return result;
        }
    }
}
