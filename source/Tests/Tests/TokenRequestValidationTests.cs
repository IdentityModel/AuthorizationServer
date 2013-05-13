using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.AuthorizationServer.Test;
using Thinktecture.AuthorizationServer.OAuth2;
using System.Net;
using System.Net.Http;
using Thinktecture.IdentityModel;
using System.Security.Claims;

namespace Tests
{
    [TestClass]
    public class TokenRequestValidationTests
    {
        IAuthorizationServerConfiguration _testConfig = new TestAuthorizationServerConfiguration();
        
        ClaimsPrincipal _codeClient = Principal.Create("Test",
                                        new Claim(ClaimTypes.Name, "codeclient"),
                                        new Claim("password", "secret"));

        ClaimsPrincipal _resourceOwnerClient = Principal.Create("Test",
                                        new Claim(ClaimTypes.Name, "roclient"),
                                        new Claim("password", "secret"));


        [TestMethod]
        public void UnknownApplication()
        {
            var controller = new TokenController(null, _testConfig)
            {
                Request = new HttpRequestMessage()
            };

            var result = controller.Post("unknown", null);

            Assert.IsTrue(result.StatusCode == HttpStatusCode.NotFound);
        }

        [TestMethod]
        [ExpectedException(typeof(TokenRequestValidationException))]
        public void NoParameters()
        {
            var validator = new RequestValidator();
            var app = _testConfig.FindApplication("test");

            var result = validator.ValidateTokenRequest(app, null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(TokenRequestValidationException))]
        public void EmptyParameters()
        {
            var validator = new RequestValidator();
            var app = _testConfig.FindApplication("test");

            var result = validator.ValidateTokenRequest(app, new TokenRequest(), _codeClient);
        }

        [TestMethod]
        public void ValidCodeGrant()
        {
            var validator = new RequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new TokenRequest
            {
                Grant_Type = OAuthConstants.GrantTypes.AuthorizationCode,
                Code = "abc"
            };

            var result = validator.ValidateTokenRequest(app, request, _codeClient);
        }

        [TestMethod]
        public void ValidPasswordGrant()
        {
            var validator = new RequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new TokenRequest
            {
                Grant_Type = OAuthConstants.GrantTypes.Password,
                UserName = "username",
                Password = "password",
                Scope = "read"
            };

            var result = validator.ValidateTokenRequest(app, request, _resourceOwnerClient);
        }

        [TestMethod]
        [ExpectedException(typeof(TokenRequestValidationException))]
        public void AnonymousCodeGrant()
        {
            var validator = new RequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new TokenRequest
            {
                Grant_Type = OAuthConstants.GrantTypes.AuthorizationCode,
                Code = "abc"
            };

            var result = validator.ValidateTokenRequest(app, request, Principal.Anonymous);
        }

        [TestMethod]
        [ExpectedException(typeof(TokenRequestValidationException))]
        public void MissingGrantTypeWithCode()
        {
            var validator = new RequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new TokenRequest
            {
                Code = "abc"
            };

            var result = validator.ValidateTokenRequest(app, request, _codeClient);
        }

        [TestMethod]
        [ExpectedException(typeof(TokenRequestValidationException))]
        public void MissingCode()
        {
            var validator = new RequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new TokenRequest
            {
                Code = OAuthConstants.GrantTypes.AuthorizationCode
            };

            var result = validator.ValidateTokenRequest(app, request, _codeClient);
        }

        [TestMethod]
        [ExpectedException(typeof(TokenRequestValidationException))]
        public void MissingGrantTypeWithPassword()
        {
            var validator = new RequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new TokenRequest
            {
                UserName = "username",
                Password = "password",
                Scope = "read"
            };

            var result = validator.ValidateTokenRequest(app, request, _resourceOwnerClient);
        }

        [TestMethod]
        [ExpectedException(typeof(TokenRequestValidationException))]
        public void UnknownGrantType()
        {
            var validator = new RequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new TokenRequest
            {
                Grant_Type = "unknown"
            };

            var result = validator.ValidateTokenRequest(app, request, _resourceOwnerClient);
        }

        [TestMethod]
        [ExpectedException(typeof(TokenRequestValidationException))]
        public void UnauthorizedCodeGrant()
        {
            var validator = new RequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new TokenRequest
            {
                Grant_Type = OAuthConstants.GrantTypes.AuthorizationCode,
                Code = "abc"
            };

            var result = validator.ValidateTokenRequest(app, request, _resourceOwnerClient);
        }

        [TestMethod]
        [ExpectedException(typeof(TokenRequestValidationException))]
        public void UnauthorizedPasswordGrant()
        {
            var validator = new RequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new TokenRequest
            {
                Grant_Type = OAuthConstants.GrantTypes.Password,
                UserName = "username",
                Password = "password",
                Scope = "read"
            };

            var result = validator.ValidateTokenRequest(app, request, _codeClient);
        }

        [TestMethod]
        [ExpectedException(typeof(TokenRequestValidationException))]
        public void MissingScope()
        {
            var validator = new RequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new TokenRequest
            {
                Grant_Type = OAuthConstants.GrantTypes.Password,
                UserName = "username",
                Password = "password",
            };

            var result = validator.ValidateTokenRequest(app, request, _resourceOwnerClient);
        }

        [TestMethod]
        [ExpectedException(typeof(TokenRequestValidationException))]
        public void UnknownScope()
        {
            var validator = new RequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new TokenRequest
            {
                Grant_Type = OAuthConstants.GrantTypes.Password,
                UserName = "username",
                Password = "password",
                Scope = "unknown"
            };

            var result = validator.ValidateTokenRequest(app, request, _resourceOwnerClient);
        }

        [TestMethod]
        [ExpectedException(typeof(TokenRequestValidationException))]
        public void UnauthorizedScopeSingle()
        {
            var validator = new RequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new TokenRequest
            {
                Grant_Type = OAuthConstants.GrantTypes.Password,
                UserName = "username",
                Password = "password",
                Scope = "delete"
            };

            var result = validator.ValidateTokenRequest(app, request, _resourceOwnerClient);
        }

        [TestMethod]
        [ExpectedException(typeof(TokenRequestValidationException))]
        public void UnauthorizedScopeMultiple()
        {
            var validator = new RequestValidator();
            var app = _testConfig.FindApplication("test");
            var request = new TokenRequest
            {
                Grant_Type = OAuthConstants.GrantTypes.Password,
                UserName = "username",
                Password = "password",
                Scope = "read delete"
            };

            var result = validator.ValidateTokenRequest(app, request, _resourceOwnerClient);
        }
    }
}