using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Claims;
using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.AuthorizationServer.OAuth2;
using Thinktecture.IdentityModel;

namespace Thinktecture.AuthorizationServer.Test
{
    [TestClass]
    public class TokenRequest_Validation_Password
    {
        IAuthorizationServerConfiguration _testConfig;
        ClaimsPrincipal _client;

        [TestInitialize]
        public void Init()
        {
            DataProtectection.Instance = new NoProtection();
    
            _testConfig = new TestAuthorizationServerConfiguration();
            _client = Principal.Create(
                "Test",
                new Claim(ClaimTypes.Name, "roclient"),
                new Claim("password", "secret"));
        }

        [TestMethod]
        public void ValidSingleScope()
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

            var result = validator.Validate(app, request, _client);
        }

        [TestMethod]
        public void MissingScope()
        {
            var validator = new TokenRequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new TokenRequest
            {
                Grant_Type = OAuthConstants.GrantTypes.Password,
                UserName = "username",
                Password = "password",
            };

            try
            {
                var result = validator.Validate(app, request, _client);
            }
            catch (TokenRequestValidationException ex)
            {
                Assert.AreEqual(OAuthConstants.Errors.InvalidScope, ex.OAuthError);
                return;
            }

            Assert.Fail("No exception thrown.");
        }

        [TestMethod]
        public void UnknownScope()
        {
            var validator = new TokenRequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new TokenRequest
            {
                Grant_Type = OAuthConstants.GrantTypes.Password,
                UserName = "username",
                Password = "password",
                Scope = "unknown"
            };

            try
            {
                var result = validator.Validate(app, request, _client);
            }
            catch (TokenRequestValidationException ex)
            {
                Assert.AreEqual(OAuthConstants.Errors.InvalidScope, ex.OAuthError);
                return;
            }

            Assert.Fail("No exception thrown.");
        }

        [TestMethod]
        public void UnauthorizedScopeSingle()
        {
            var validator = new TokenRequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new TokenRequest
            {
                Grant_Type = OAuthConstants.GrantTypes.Password,
                UserName = "username",
                Password = "password",
                Scope = "delete"
            };

            try
            {
                var result = validator.Validate(app, request, _client);
            }
            catch (TokenRequestValidationException ex)
            {
                Assert.AreEqual(OAuthConstants.Errors.InvalidScope, ex.OAuthError);
                return;
            }

            Assert.Fail("No exception thrown.");
        }

        [TestMethod]
        public void UnauthorizedScopeMultiple()
        {
            var validator = new TokenRequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new TokenRequest
            {
                Grant_Type = OAuthConstants.GrantTypes.Password,
                UserName = "username",
                Password = "password",
                Scope = "read delete"
            };

            try
            {
                var result = validator.Validate(app, request, _client);
            }
            catch (TokenRequestValidationException ex)
            {
                Assert.AreEqual(OAuthConstants.Errors.InvalidScope, ex.OAuthError);
                return;
            }

            Assert.Fail("No exception thrown.");
        }

        [TestMethod]
        public void MissingResourceOwnerUserName()
        {
            var validator = new TokenRequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new TokenRequest
            {
                Grant_Type = OAuthConstants.GrantTypes.Password,
                UserName = "username",
                Scope = "read"
            };

            try
            {
                var result = validator.Validate(app, request, _client);
            }
            catch (TokenRequestValidationException ex)
            {
                Assert.AreEqual(OAuthConstants.Errors.InvalidGrant, ex.OAuthError);
                return;
            }

            Assert.Fail("No exception thrown.");
        }

        [TestMethod]
        public void MissingResourceOwnerPassword()
        {
            var validator = new TokenRequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new TokenRequest
            {
                Grant_Type = OAuthConstants.GrantTypes.Password,
                UserName = "username",
                Scope = "read"
            };

            try
            {
                var result = validator.Validate(app, request, _client);
            }
            catch (TokenRequestValidationException ex)
            {
                Assert.AreEqual(OAuthConstants.Errors.InvalidGrant, ex.OAuthError);
                return;
            }

            Assert.Fail("No exception thrown.");
        }

        [TestMethod]
        public void UnauthorizedCodeGrant()
        {
            TestTokenHandleManager handleManager =
                new TestTokenHandleManager("abc", "codeclient", "https://validredirect");
            
            var validator = new TokenRequestValidator(handleManager);
            var app = _testConfig.FindApplication("test");
            var request = new TokenRequest
            {
                Grant_Type = OAuthConstants.GrantTypes.AuthorizationCode,
            };

            try
            {
                var result = validator.Validate(app, request, _client);
            }
            catch (TokenRequestValidationException ex)
            {
                Assert.AreEqual(OAuthConstants.Errors.UnauthorizedClient, ex.OAuthError);
                return;
            }

            Assert.Fail("No exception thrown.");
        }

        [TestMethod]
        public void UnauthorizedClientCredentialsGrant()
        {
            TestTokenHandleManager handleManager =
                new TestTokenHandleManager("abc", "codeclient", "https://validredirect");

            var validator = new TokenRequestValidator(handleManager);
            var app = _testConfig.FindApplication("test");
            var request = new TokenRequest
            {
                Grant_Type = OAuthConstants.GrantTypes.ClientCredentials,
            };

            try
            {
                var result = validator.Validate(app, request, _client);
            }
            catch (TokenRequestValidationException ex)
            {
                Assert.AreEqual(OAuthConstants.Errors.UnauthorizedClient, ex.OAuthError);
                return;
            }

            Assert.Fail("No exception thrown.");
        }

        [TestMethod]
        public void UnauthorizedRefreshTokenGrant()
        {
            TestTokenHandleManager handleManager =
                new TestTokenHandleManager("abc", "codeclient", "https://validredirect");

            var validator = new TokenRequestValidator(handleManager);
            var app = _testConfig.FindApplication("test");
            var request = new TokenRequest
            {
                Grant_Type = OAuthConstants.GrantTypes.RefreshToken,
            };

            try
            {
                var result = validator.Validate(app, request, _client);
            }
            catch (TokenRequestValidationException ex)
            {
                Assert.AreEqual(OAuthConstants.Errors.UnauthorizedClient, ex.OAuthError);
                return;
            }

            Assert.Fail("No exception thrown.");
        }
    }
}