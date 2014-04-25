using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Thinktecture.AuthorizationServer;
using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.AuthorizationServer.Models;
using Thinktecture.AuthorizationServer.OAuth2;
using Thinktecture.IdentityModel;

namespace Tests.Token_Controller
{
    [TestClass]
    public class TokenController_Post
    {
        TokenController _TokenController;
        Client _Client;

        [TestInitialize]
        public void Init()
        {
            DataProtectection.Instance = new NoProtection();
            var globalConfiguration = new GlobalConfiguration() { Issuer = "Test Issuer" };

            var rocv = new Mock<IResourceOwnerCredentialValidation>();
            var config = new Mock<IAuthorizationServerConfiguration>();
            var handleManager = new Mock<IStoredGrantManager>();
            var assertionGrantValidator = new Mock<IAssertionGrantValidation>();
            var clientManager = new Mock<IClientManager>();

            var tokenService = new TokenService(config.Object);


            #region Setup Test Client
            string secret = "12345678";
            byte[] encodedByte = System.Text.ASCIIEncoding.ASCII.GetBytes(secret);
            string base64EncodedSecret = Convert.ToBase64String(encodedByte);
            var _Client = new Client()
            {
                ClientId = "MobileAppShop",
                ClientSecret = base64EncodedSecret,
                Flow = OAuthFlow.ResourceOwner,
            };

            #endregion

            #region Setup Mocking Objects

            // IAuthorizationServerConfiguration
            config.Setup(x => x.FindApplication(It.IsNotNull<string>()))
                .Returns((string name) =>
                {
                    var scope = new Scope();
                    scope.Name = "read";
                    scope.AllowedClients = new List<Client>();
                    scope.AllowedClients.Add(_Client);
                    var scopes = new List<Scope>();
                    scopes.Add(scope);

                    string symmetricKey = "C33333333333333333333333335=";
                    byte[] keybytes = Convert.FromBase64String(symmetricKey);
                    SecurityKey securityKey = new InMemorySymmetricSecurityKey(keybytes);
                    return new Application()
                    {
                        Name = name,
                        Scopes = scopes,
                        Audience = "Test Audience",
                        TokenLifetime = 1
                    };
                });
            config.Setup(x => x.GlobalConfiguration).Returns(() => globalConfiguration);

            // IClientManager
            clientManager.Setup(x => x.Get(It.IsNotNull<string>()))
                .Returns((string clientId) =>
                {
                    return _Client;
                });

            // IResourceOwnerCredentialValidation
            rocv.Setup(x => x.Validate(It.IsNotNull<string>(), It.IsNotNull<string>()))
                .Returns((string username, string password) =>
                {
                    Claim[] claims = { new Claim("Username", username) };
                    return Principal.Create("Test", claims);
                });

            #endregion

            _TokenController = new TokenController(
                rocv.Object,
                config.Object,
                handleManager.Object,
                assertionGrantValidator.Object,
                tokenService,
                clientManager.Object);
            _TokenController.Request = new HttpRequestMessage();
            _TokenController.Request.SetConfiguration(new HttpConfiguration());
        }

        [TestMethod]
        public void Create_Token_With_Resource_Owner_Flow()
        {
            Claim[] claims = { new Claim("client_id", "MobileAppShop"), new Claim("secret", "12345678") };
            ClaimsPrincipal claimsPrinciple = Principal.Create("Test", claims);

            Thread.CurrentPrincipal = claimsPrinciple;

            TokenRequest tokenRequest = new TokenRequest()
            {
                Grant_Type = "password",
                UserName = "JohnSmith",
                Password = "12345678",
                Scope = "read",
            };

            var response = _TokenController.Post("Application 1", tokenRequest);
            TokenResponse tokenResponse;
            response.TryGetContentValue<TokenResponse>(out tokenResponse);

            Assert.IsTrue(response.IsSuccessStatusCode == true);
            Assert.IsFalse (string.IsNullOrEmpty(tokenResponse.AccessToken));
        }
    }
}
