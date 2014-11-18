using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.AuthorizationServer.OAuth2;
using Thinktecture.IdentityModel;

namespace Thinktecture.AuthorizationServer.Test
{
    [TestClass]
    public class TokenRequest_Validation_General
    {
        IAuthorizationServerConfiguration _testConfig;

        [TestInitialize]
        public void Init()
        {
            DataProtectection.Instance = new NoProtection();
            _testConfig = new TestAuthorizationServerConfiguration();
        }

        [TestMethod]
        public void UnknownApplication()
        {
            var controller = new TokenController(null, _testConfig, null, null, null)
            {
                Request = new HttpRequestMessage()
            };

            var result = controller.Post("unknown", null);

            Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
        }

        [TestMethod]
        public void MissingClientPassword()
        {
            var validator = new TokenRequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new TokenRequest
            {
                Grant_Type = OAuthConstants.GrantTypes.Password,
                UserName = "username",
                Password = "password",
                Scope = "read"
            };

            try
            {
                var client = Principal.Create("Test",
                                new Claim(ClaimTypes.Name, "codeclient"));

                var result = validator.Validate(app, request, client);
            }
            catch (TokenRequestValidationException ex)
            {
                Assert.AreEqual(OAuthConstants.Errors.InvalidClient, ex.OAuthError);
                return;
            }

            Assert.Fail("No exception thrown.");
        }

        [TestMethod]
        public void MissingClientId()
        {
            var validator = new TokenRequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new TokenRequest
            {
                Grant_Type = OAuthConstants.GrantTypes.Password,
                UserName = "username",
                Password = "password",
                Scope = "read"
            };

            try
            {
                var client = Principal.Create("Test",
                                new Claim("password", "secret"));

                var result = validator.Validate(app, request, client);
            }
            catch (TokenRequestValidationException ex)
            {
                Assert.AreEqual(OAuthConstants.Errors.InvalidClient, ex.OAuthError);
                return;
            }

            Assert.Fail("No exception thrown.");
        }

        [TestMethod]
        public void NoClientClaims()
        {
            var validator = new TokenRequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new TokenRequest
            {
                Grant_Type = OAuthConstants.GrantTypes.Password,
                UserName = "username",
                Password = "password",
                Scope = "read"
            };

            try
            {
                var result = validator.Validate(app, request, null);
            }
            catch (TokenRequestValidationException ex)
            {
                Assert.AreEqual(OAuthConstants.Errors.InvalidClient, ex.OAuthError);
                return;
            }

            Assert.Fail("No exception thrown.");
        }

        [TestMethod]
        public void NoParameters()
        {
            var validator = new TokenRequestValidator();
            var app = _testConfig.FindApplication("test");

            try
            {
                var result = validator.Validate(app, null, null);
            }
            catch (TokenRequestValidationException ex)
            {
                Assert.AreEqual(OAuthConstants.Errors.InvalidRequest, ex.OAuthError);
                return;
            }

            Assert.Fail("No exception thrown.");
        }

        [TestMethod]
        public void EmptyParameters()
        {
            var validator = new TokenRequestValidator();
            var app = _testConfig.FindApplication("test");

            try
            {
                var result = validator.Validate(app, new TokenRequest(), null);
            }
            catch (TokenRequestValidationException ex)
            {
                Assert.AreEqual(OAuthConstants.Errors.UnsupportedGrantType, ex.OAuthError);
                return;
            }

            Assert.Fail("No exception thrown.");
        }

        [TestMethod]
        public void MissingGrantType()
        {
            var validator = new TokenRequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new TokenRequest
            {
                Code = "abc"
            };

            try
            {
                var result = validator.Validate(app, request, null);
            }
            catch (TokenRequestValidationException ex)
            {
                Assert.AreEqual(OAuthConstants.Errors.UnsupportedGrantType, ex.OAuthError);
                return;
            }

            Assert.Fail("No exception thrown.");
        }

        [TestMethod]
        public void UnknownGrantType()
        {
            var validator = new TokenRequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new TokenRequest
            {
                Grant_Type = "unknown"
            };

            try
            {
                var result = validator.Validate(app, request, null);
            }
            catch (TokenRequestValidationException ex)
            {
                Assert.AreEqual(OAuthConstants.Errors.UnsupportedGrantType, ex.OAuthError);
                return;
            }

            Assert.Fail("No exception thrown.");
        }

        [TestMethod]
        public void DisabledClient()
        {
            var validator = new TokenRequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new TokenRequest
            {
                Grant_Type = OAuthConstants.GrantTypes.ClientCredentials
            };

            try
            {
                var client = Principal.Create("Test",
                new Claim("client_id", "disabledclient"),
                new Claim("secret", "secret"));

                var result = validator.Validate(app, request, client);
            }
            catch (TokenRequestValidationException ex)
            {
                Assert.AreEqual(OAuthConstants.Errors.InvalidClient, ex.OAuthError);
                return;
            }

            Assert.Fail("No exception thrown.");
        }
    }
}