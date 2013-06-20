using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.AuthorizationServer.OAuth2;

namespace Thinktecture.AuthorizationServer.Test
{
    [TestClass]
    public class AuthorizeRequest_Validation_Token
    {
        IAuthorizationServerConfiguration _testConfig = new TestAuthorizationServerConfiguration();

        [TestMethod]
        public void ValidRequestSingleScope()
        {
            var validator = new AuthorizeRequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new AuthorizeRequest
            {
                client_id = "implicitclient",
                response_type = "token",
                scope = "read",
                redirect_uri = "https://test2.local"
            };

            var result = validator.Validate(app, request);
        }

        [TestMethod]
        public void ValidRequestMultipleScope()
        {
            var validator = new AuthorizeRequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new AuthorizeRequest
            {
                client_id = "implicitclient",
                response_type = "token",
                scope = "read browse",
                redirect_uri = "https://test2.local"
            };

            var result = validator.Validate(app, request);
        }

        [TestMethod]
        public void UnauthorizedRedirectUri()
        {
            var validator = new AuthorizeRequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new AuthorizeRequest
            {
                client_id = "implicitclient",
                response_type = "token",
                scope = "read",
                redirect_uri = "https://unauthorized.com"
            };

            try
            {
                var result = validator.Validate(app, request);
            }
            catch (AuthorizeRequestResourceOwnerException ex)
            {
                // todo: check error code
                return;
            }

            Assert.Fail("No exception thrown.");
        }

        [TestMethod]
        public void UnauthorizedResponseType()
        {
            var validator = new AuthorizeRequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new AuthorizeRequest
            {
                client_id = "implicitclient",
                response_type = "code",
                scope = "read",
                redirect_uri = "https://test2.local"
            };

            try
            {
                var result = validator.Validate(app, request);
            }
            catch (AuthorizeRequestClientException ex)
            {
                Assert.AreEqual(OAuthConstants.Errors.UnsupportedResponseType, ex.Error);
                return;
            }

            Assert.Fail("No exception thrown.");
        }

        

        [TestMethod]
        public void UnauthorizedScopeSingle()
        {
            var validator = new AuthorizeRequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new AuthorizeRequest
            {
                client_id = "implicitclient",
                response_type = "token",
                scope = "write",
                redirect_uri = "https://test2.local"
            };

            try
            {
                var result = validator.Validate(app, request);
            }
            catch (AuthorizeRequestClientException ex)
            {
                Assert.AreEqual(OAuthConstants.Errors.InvalidScope, ex.Error);
                return;
            }

            Assert.Fail("No exception thrown.");
        }

        [TestMethod]
        public void UnauthorizedScopeMultiple()
        {
            var validator = new AuthorizeRequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new AuthorizeRequest
            {
                client_id = "implicitclient",
                response_type = "token",
                scope = "read write",
                redirect_uri = "https://test2.local"
            };

            try
            {
                var result = validator.Validate(app, request);
            }
            catch (AuthorizeRequestClientException ex)
            {
                Assert.AreEqual(OAuthConstants.Errors.InvalidScope, ex.Error);
                return;
            }

            Assert.Fail("No exception thrown.");
        }
    }
}