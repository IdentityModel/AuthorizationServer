using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Claims;
using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.AuthorizationServer.Models;
using Thinktecture.AuthorizationServer.OAuth2;
using Thinktecture.IdentityModel;

namespace Thinktecture.AuthorizationServer.Test
{
    [TestClass]
    public class TokenRequest_Validation_RefreshToken
    {
        IAuthorizationServerConfiguration _testConfig;
        ClaimsPrincipal _client;
        TestTokenHandleManager _handleManager;
        TestClientManager _clientManager;

        [TestInitialize]
        public void Init()
        {
            DataProtectection.Instance = new NoProtection();

            _testConfig = new TestAuthorizationServerConfiguration();
            _client = Principal.Create(
                "Test",
                new Claim("client_id", "codeclient"),
                new Claim("secret", "secret"));
            _handleManager = new TestTokenHandleManager(
                "abc", 
                "codeclient", 
                "https://validredirect");
            _clientManager = new TestClientManager() { Id = "codeclient", Secret = "secret", OAuthFlow = OAuthFlow.Code };

        }

        [TestMethod]
        public void ValidRequest()
        {
            var validator = new TokenRequestValidator(_handleManager, _clientManager);
            var app = _testConfig.FindApplication("test");
            var request = new TokenRequest
            {
                Grant_Type = OAuthConstants.GrantTypes.RefreshToken,
                Refresh_Token = "abc"
            };

            var result = validator.Validate(app, request, _client);
        }

        [TestMethod]
        public void ExpiredRefreshToken()
        {
            TestTokenHandleManager handleManager =
                new TestTokenHandleManager("abc", "codeclient", "https://validredirect", expired: true);

            var validator = new TokenRequestValidator(handleManager, _clientManager);
            var app = _testConfig.FindApplication("test");
            var request = new TokenRequest
            {
                Grant_Type = OAuthConstants.GrantTypes.RefreshToken,
                Refresh_Token = "abc"
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
            var validator = new TokenRequestValidator(_handleManager, _clientManager);
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
                Assert.AreEqual(OAuthConstants.Errors.InvalidGrant, ex.OAuthError);
                return;
            }

            Assert.Fail("No exception thrown.");
        }

        [TestMethod]
        public void InvalidCode()
        {
            var validator = new TokenRequestValidator(_handleManager, _clientManager);
            var app = _testConfig.FindApplication("test");
            var request = new TokenRequest
            {
                Grant_Type = OAuthConstants.GrantTypes.RefreshToken,
                Refresh_Token = "xyz",
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
        public void InvalidCodeToClientBinding()
        {
            var handleManager =
                new TestTokenHandleManager("abc", "someotherclient", "https://validredirect");

            var validator = new TokenRequestValidator(handleManager, _clientManager);
            var app = _testConfig.FindApplication("test");
            var request = new TokenRequest
            {
                Grant_Type = OAuthConstants.GrantTypes.RefreshToken,
                Refresh_Token = "abc",
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
    }
}