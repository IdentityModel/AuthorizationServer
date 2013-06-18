using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.AuthorizationServer.OAuth2;

namespace Thinktecture.AuthorizationServer.Test
{
    [TestClass]
    public class Authorize_Request_Validation_Code
    {
        IAuthorizationServerConfiguration _testConfig = new TestAuthorizationServerConfiguration();

        [TestMethod]
        public void ValidRequestSingleScope()
        {
            var validator = new AuthorizeRequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new AuthorizeRequest
            {
                client_id = "codeclient",
                response_type = "code",
                scope = "read",
                redirect_uri = "https://prod.local"
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
                client_id = "codeclient",
                response_type = "code",
                scope = "read search",
                redirect_uri = "https://prod.local"
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
                client_id = "codeclient",
                response_type = "code",
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
                client_id = "codeclient",
                response_type = "token",
                scope = "read",
                redirect_uri = "https://prod.local"
            };

            try
            {
                var result = validator.Validate(app, request);
            }
            catch (AuthorizeRequestClientException ex)
            {
                Assert.IsTrue(ex.Error == OAuthConstants.Errors.UnsupportedResponseType);
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
                client_id = "codeclient",
                response_type = "code",
                scope = "write",
                redirect_uri = "https://prod.local"
            };

            try
            {
                var result = validator.Validate(app, request);
            }
            catch (AuthorizeRequestClientException ex)
            {
                Assert.IsTrue(ex.Error == OAuthConstants.Errors.InvalidScope);
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
                client_id = "codeclient",
                response_type = "code",
                scope = "read write",
                redirect_uri = "https://prod.local"
            };

            try
            {
                var result = validator.Validate(app, request);
            }
            catch (AuthorizeRequestClientException ex)
            {
                Assert.IsTrue(ex.Error == OAuthConstants.Errors.InvalidScope);
                return;
            }

            Assert.Fail("No exception thrown.");
        }
    }
}