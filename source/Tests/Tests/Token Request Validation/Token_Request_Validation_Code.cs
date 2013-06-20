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
    public class Token_Request_Validation_Code
    {
        IAuthorizationServerConfiguration _testConfig = new TestAuthorizationServerConfiguration();

        ClaimsPrincipal _codeClient = Principal.Create("Test",
                                        new Claim(ClaimTypes.Name, "codeclient"),
                                        new Claim("password", "secret"));

        TestTokenHandleManager _handleManager = 
            new TestTokenHandleManager("abc", "codeclient", "https://validredirect");

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

            var result = validator.Validate(app, request, _codeClient);
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
                var result = validator.Validate(app, request, _codeClient);
            }
            catch (TokenRequestValidationException ex)
            {
                Assert.IsTrue(ex.OAuthError == OAuthConstants.Errors.InvalidRequest);
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
                var result = validator.Validate(app, request, _codeClient);
            }
            catch (TokenRequestValidationException ex)
            {
                Assert.IsTrue(ex.OAuthError == OAuthConstants.Errors.InvalidRequest);
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
                var result = validator.Validate(app, request, _codeClient);
            }
            catch (TokenRequestValidationException ex)
            {
                Assert.IsTrue(ex.OAuthError == OAuthConstants.Errors.InvalidGrant);
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
                var result = validator.Validate(app, request, _codeClient);
            }
            catch (TokenRequestValidationException ex)
            {
                Assert.IsTrue(ex.OAuthError == OAuthConstants.Errors.InvalidGrant);
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
                Assert.IsTrue(ex.OAuthError == OAuthConstants.Errors.InvalidClient);
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
                var result = validator.Validate(app, request, _codeClient);
            }
            catch (TokenRequestValidationException ex)
            {
                Assert.IsTrue(ex.OAuthError == OAuthConstants.Errors.InvalidGrant);
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
                var result = validator.Validate(app, request, _codeClient);
            }
            catch (TokenRequestValidationException ex)
            {
                Assert.IsTrue(ex.OAuthError == OAuthConstants.Errors.UnauthorizedClient);
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
                var result = validator.Validate(app, request, _codeClient);
            }
            catch (TokenRequestValidationException ex)
            {
                Assert.IsTrue(ex.OAuthError == OAuthConstants.Errors.UnauthorizedClient);
                return;
            }

            Assert.Fail("No exception thrown.");
        }
    }
}