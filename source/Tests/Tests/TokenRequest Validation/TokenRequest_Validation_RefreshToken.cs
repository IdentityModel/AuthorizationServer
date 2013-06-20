using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.AuthorizationServer.OAuth2;
using Thinktecture.AuthorizationServer.Test;
using Thinktecture.IdentityModel;

namespace Tests
{
    [TestClass]
    public class TokenRequest_Validation_RefreshToken
    {
        IAuthorizationServerConfiguration _testConfig = new TestAuthorizationServerConfiguration();

        ClaimsPrincipal _client = Principal.Create("Test",
                                        new Claim(ClaimTypes.Name, "codeclient"),
                                        new Claim("password", "secret"));
        
        TestTokenHandleManager _handleManager =
            new TestTokenHandleManager("abc", "codeclient", "https://validredirect");

        [TestMethod]
        public void ValidRequest()
        {
            var validator = new TokenRequestValidator(_handleManager);
            var app = _testConfig.FindApplication("test");
            var request = new TokenRequest
            {
                Grant_Type = OAuthConstants.GrantTypes.RefreshToken,
                Refresh_Token = "abc"
            };

            var result = validator.Validate(app, request, _client);
        }

        [TestMethod]
        public void MissingCode()
        {
            var validator = new TokenRequestValidator(_handleManager);
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
            var validator = new TokenRequestValidator(_handleManager);
            var app = _testConfig.FindApplication("test");
            var request = new TokenRequest
            {
                Grant_Type = OAuthConstants.GrantTypes.RefreshToken,
                Code = "xyz",
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

            var validator = new TokenRequestValidator(handleManager);
            var app = _testConfig.FindApplication("test");
            var request = new TokenRequest
            {
                Grant_Type = OAuthConstants.GrantTypes.RefreshToken,
                Code = "abc",
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