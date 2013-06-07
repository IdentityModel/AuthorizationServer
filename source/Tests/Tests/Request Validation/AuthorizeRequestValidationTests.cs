using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.AuthorizationServer.OAuth2;

namespace Thinktecture.AuthorizationServer.Test
{
    [TestClass]
    public class AuthorizeRequest_Validation
    {
        IAuthorizationServerConfiguration _testConfig = new TestAuthorizationServerConfiguration();

        [TestMethod]
        public void UnknownApplication()
        {
            var controller = new AuthorizeController(null, _testConfig);
            var result = controller.Index("unknown", null);

            Assert.IsTrue(result is HttpNotFoundResult);
        }

        [TestMethod]
        [ExpectedException(typeof(AuthorizeRequestResourceOwnerException))]
        public void NoParameters()
        {
            var validator = new AuthorizeRequestValidator();
            var app = _testConfig.FindApplication("test");

            var result = validator.Validate(app, null);
        }

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
        [ExpectedException(typeof(AuthorizeRequestResourceOwnerException))]
        public void MissingRedirectUri()
        {
            var validator = new AuthorizeRequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new AuthorizeRequest
            {
                client_id = "codeclient",
                response_type = "code",
                scope = "read"
            };

            var result = validator.Validate(app, request);
        }

        [TestMethod]
        [ExpectedException(typeof(AuthorizeRequestResourceOwnerException))]
        public void MalformedRedirectUri1()
        {
            var validator = new AuthorizeRequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new AuthorizeRequest
            {
                client_id = "codeclient",
                response_type = "code",
                scope = "read",
                redirect_uri = "https:/prod.local"
            };

            var result = validator.Validate(app, request);
        }

        [TestMethod]
        [ExpectedException(typeof(AuthorizeRequestResourceOwnerException))]
        public void MalformedRedirectUri2()
        {
            var validator = new AuthorizeRequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new AuthorizeRequest
            {
                client_id = "codeclient",
                response_type = "code",
                scope = "read",
                redirect_uri = "malformed"
            };

            var result = validator.Validate(app, request);
        }

        [TestMethod]
        [ExpectedException(typeof(AuthorizeRequestResourceOwnerException))]
        public void InvalidRedirectUri()
        {
            var validator = new AuthorizeRequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new AuthorizeRequest
            {
                client_id = "codeclient",
                response_type = "code",
                scope = "read",
                redirect_uri = "https://invalid.com"
            };

            var result = validator.Validate(app, request);
        }

        [TestMethod]
        [ExpectedException(typeof(AuthorizeRequestClientException))]
        public void NonSslRedirectUri()
        {
            var validator = new AuthorizeRequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new AuthorizeRequest
            {
                client_id = "codeclient",
                response_type = "code",
                scope = "read",
                redirect_uri = "http://prod.local"
            };

            var result = validator.Validate(app, request);
        }

        [TestMethod]
        [ExpectedException(typeof(AuthorizeRequestResourceOwnerException))]
        public void MissingClientId()
        {
            var validator = new AuthorizeRequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new AuthorizeRequest
            {
                response_type = "code",
                scope = "read",
                redirect_uri = "https://prod.local"
            };

            var result = validator.Validate(app, request);
        }

        [TestMethod]
        [ExpectedException(typeof(AuthorizeRequestResourceOwnerException))]
        public void UnknownClientId()
        {
            var validator = new AuthorizeRequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new AuthorizeRequest
            {
                client_id = "unknown",
                response_type = "code",
                scope = "read",
                redirect_uri = "https://prod.local"
            };

            var result = validator.Validate(app, request);
        }

        [TestMethod]
        [ExpectedException(typeof(AuthorizeRequestClientException))]
        public void MissingResponseType()
        {
            var validator = new AuthorizeRequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new AuthorizeRequest
            {
                client_id = "codeclient",
                scope = "read",
                redirect_uri = "https://prod.local"
            };

            var result = validator.Validate(app, request);
        }

        [TestMethod]
        [ExpectedException(typeof(AuthorizeRequestClientException))]
        public void UnsupportedResponseType()
        {
            var validator = new AuthorizeRequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new AuthorizeRequest
            {
                client_id = "codeclient",
                response_type = "unsupported",
                scope = "read",
                redirect_uri = "https://prod.local"
            };

            var result = validator.Validate(app, request);
        }

        [TestMethod]
        [ExpectedException(typeof(AuthorizeRequestClientException))]
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

            var result = validator.Validate(app, request);
        }

        [TestMethod]
        [ExpectedException(typeof(AuthorizeRequestClientException))]
        public void MissingScope()
        {
            var validator = new AuthorizeRequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new AuthorizeRequest
            {
                client_id = "codeclient",
                response_type = "code",
                redirect_uri = "https://prod.local"
            };

            var result = validator.Validate(app, request);
        }

        [TestMethod]
        [ExpectedException(typeof(AuthorizeRequestClientException))]
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

            var result = validator.Validate(app, request);
        }

        [TestMethod]
        [ExpectedException(typeof(AuthorizeRequestClientException))]
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

            var result = validator.Validate(app, request);
        }
    }
}