/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System;
using System.IdentityModel.Selectors;
using System.IdentityModel.Services;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.IdentityModel.Extensions;
using Thinktecture.IdentityModel.WSTrust;

namespace Thinktecture.AuthorizationServer.OAuth2
{
    public class WSTrustResourceOwnerCredentialValidation : IResourceOwnerCredentialValidation
    {
        string _address;
        string _realm;
        string _issuerThumbprint;
        string _decryptionThumbprint;

        public WSTrustResourceOwnerCredentialValidation(string address, string realm, string issuerThumbprint, string decryptionThumbprint="")
        {
            _address = address;
            _realm = realm;
            _issuerThumbprint = issuerThumbprint;
            _decryptionThumbprint = decryptionThumbprint;
        }

        public ClaimsPrincipal Validate(string userName, string password)
        {
            var binding = new UserNameWSTrustBinding(SecurityMode.TransportWithMessageCredential);
            var credentials = new ClientCredentials();
            credentials.UserName.UserName = userName;
            credentials.UserName.Password = password;

            GenericXmlSecurityToken genericToken;
            genericToken = WSTrustClient.Issue(
                new EndpointAddress(_address),
                new EndpointAddress(_realm),
                binding,
                credentials) as GenericXmlSecurityToken;

            var config = new SecurityTokenHandlerConfiguration();
            config.AudienceRestriction.AllowedAudienceUris.Add(new Uri(_realm));

            config.CertificateValidationMode = X509CertificateValidationMode.None;
            config.CertificateValidator = X509CertificateValidator.None;


            //var registry = new ConfigurationBasedIssuerNameRegistry();
            //registry.AddTrustedIssuer(_issuerThumbprint, _address);

            CustomNameRegistry registry = new CustomNameRegistry();
            registry.AddTrustedIssuer(_issuerThumbprint, _address);
            


            config.IssuerNameRegistry = registry;
            var handler = SecurityTokenHandlerCollection.CreateDefaultSecurityTokenHandlerCollection(config);
            ClaimsPrincipal principal;


            SecurityToken token;
            if (_decryptionThumbprint != "")
            {
                var cert = IdentityModel.X509.LocalMachine.My.Thumbprint.Find(_decryptionThumbprint).GetEnumerator();
                cert.MoveNext();
                token = genericToken.ToSecurityToken(cert.Current);
            }
            else {
                token = genericToken.ToSecurityToken();
            }


            principal = new ClaimsPrincipal(handler.ValidateToken(token));
            
            

            Tracing.Information("Successfully requested token for user via WS-Trust");
            return FederatedAuthentication.FederationConfiguration.IdentityConfiguration.ClaimsAuthenticationManager.Authenticate("ResourceOwnerPasswordValidation", principal);
        }
    }
}
