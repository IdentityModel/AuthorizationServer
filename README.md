# Thinktecture AuthorizationServer

AuthorizationServer is the foundation for implementing application and API authorization.
As a first step, we provide an implementation of the OAuth2 authorization framework.


### Concepts

We support the following primitives:

**Applications**
Applications are containers for settings (token lifetime, key material, audience…) and scopes. Every application gets its own entry point in the URL structure, e.g. ../myapp/oauth/authorize and ../myapp/oauth/token.

**Scopes**
Scopes represent permissions a client can ask for. They will be shown on consent screens, so the resource owner can grant (or deny) access. A scope also defines which clients can request it.

**Clients**
A client has a client ID and a secret. A client can use exactly one OAuth2 flow to request tokens (code, implicit, resource owner credentials, client credentials). A client has a list of allowed redirect URIs for flows that require a callback.

**Access Tokens**
An access token will contain JWT standard claims like iss (issuer), aud (audience), nbf (not before), exp (expiration). In addition it will contain information about the subject (sub claim), the client that requested the token as well as the requested scopes.

**Flows** 
We support all OAuth2 flows like authorization code, implicit, resource owner and client credentials flow. In addition you can extend the token endpoint to support assertion flow, which enables delegation and federation scenarios.

### Architecture

AS deliberately doesn't do authentication. It solely focuses on authorization. The default configuration assumes AS is a relying party to some WS-Federation identity provider (e.g. IdentityServer, ADFS, Windows Azure Active Directory or Azure Access Control Service). You can of course customize that in any way you want, e.g. add a local login page.

AS has only a single requirement when it comes to identity of the resource owner: the current principal must contain a claim of type "sub" (subject). You can adapt to your own claims structure using the ClaimsTransformer class in the web host project.

See the [wiki] (https://github.com/thinktecture/Thinktecture.AuthorizationServer/wiki) for more information.
