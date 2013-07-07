using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Claims;
using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.AuthorizationServer.OAuth2;
using Thinktecture.IdentityModel;

namespace Thinktecture.AuthorizationServer.Test
{
    [TestClass]
    public class TokenRequest_Validation_Code
    {
        IAuthorizationServerConfiguration _testConfig;
        ClaimsPrincipal _client;
        TestTokenHandleManager _handleManager;

        [TestInitialize]
        public void Init()
        {
            DataProtectection.Instance = new NoProtection();
            _testConfig = new TestAuthorizationServerConfiguration();

            _client = Principal.Create(
                "Test",
                new Claim(ClaimTypes.Name, "codeclient"),
                new Claim("password", "secret"));
            _handleManager = new TestTokenHandleManager(
                "abc", 
                "codeclient", 
                "https://validredirect");

        }

        [TestMethod]
        public void ValidSingleScope()
        {
            var validator = new TokenRequestValidator(_handleManager);
            var app = _testConfig.FindApplication("test");
            var request = new TokenRequest
            {
                Grant_Type = OAuthConstants.GrantTypes.AuthorizationCode,
                Code = "abc",
                Redirect_Uri = "https://validredirect"
            };

            var result = validator.Validate(app, request, _client);
        }

        [TestMethod]
        public void MissingRedirectUri()
        {
            var validator = new TokenRequestValidator(_handleManager);
            var app = _testConfig.FindApplication("test");
            var request = new TokenRequest
            {
                Grant_Type = OAuthConstants.GrantTypes.AuthorizationCode,
                Code = "abc",
            };

            try
            {
                var result = validator.Validate(app, request, _client);
            }
            catch (TokenRequestValidationException ex)
            {
                Assert.AreEqual(OAuthConstants.Errors.InvalidRequest, ex.OAuthError);
                return;
            }

            Assert.Fail("No exception thrown.");
        }

        [TestMethod]
        public void NonMatchingRedirectUri()
        {
            var validator = new TokenRequestValidator(_handleManager);
            var app = _testConfig.FindApplication("test");
            var request = new TokenRequest
            {
                Grant_Type = OAuthConstants.GrantTypes.AuthorizationCode,
                Code = "abc",
                Redirect_Uri = "https://invalidredirect"
            };

            try
            {
                var result = validator.Validate(app, request, _client);
            }
            catch (TokenRequestValidationException ex)
            {
                Assert.AreEqual(OAuthConstants.Errors.InvalidRequest, ex.OAuthError);
                return;
            }

            Assert.Fail("No exception thrown.");
        }

        [TestMethod]
        public void InvalidCodeToClientBinding()
        {
            var handleManager =
                new TestTokenHandleManager("abc", "someotherclient", "https://validredirect");

            var validator = new TokenRequestValidator(handleManager);
            var app = _testConfig.FindApplication("test");
            var request = new TokenRequest
            {
                Grant_Type = OAuthConstants.GrantTypes.AuthorizationCode,
                Code = "abc",
                Redirect_Uri = "https://validredirect"
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
        public void MissingCode()
        {
            var validator = new TokenRequestValidator(_handleManager);
            var app = _testConfig.FindApplication("test");
            var request = new TokenRequest
            {
                Grant_Type = OAuthConstants.GrantTypes.AuthorizationCode,
                Redirect_Uri = "https://validredirect"
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
        public void AnonymousCodeGrant()
        {
            var validator = new TokenRequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new TokenRequest
            {
                Grant_Type = OAuthConstants.GrantTypes.AuthorizationCode,
                Code = "abc",
                Redirect_Uri = "https://validredirect"
            };

            try
            {
                var result = validator.Validate(app, request, Principal.Anonymous);
            }
            catch (TokenRequestValidationException ex)
            {
                Assert.AreEqual(OAuthConstants.Errors.InvalidClient, ex.OAuthError);
                return;
            }

            Assert.Fail("No exception thrown.");
        }

        [TestMethod]
        public void InvalidCode()
        {
            var validator = new TokenRequestValidator(_handleManager);
            var app = _testConfig.FindApplication("test");
            var request = new TokenRequest
            {
                Grant_Type = OAuthConstants.GrantTypes.AuthorizationCode,
                Code = "xyz",
                Redirect_Uri = "https://validredirect"
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
        public void UnauthorizedPasswordGrant()
        {
            var validator = new TokenRequestValidator(_handleManager);
            var app = _testConfig.FindApplication("test");
            var request = new TokenRequest
            {
                Grant_Type = OAuthConstants.GrantTypes.Password,
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
        public void UnauthorizedClientCredentialGrant()
        {
            var validator = new TokenRequestValidator(_handleManager);
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
    }
}