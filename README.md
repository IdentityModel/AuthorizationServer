# Thinktecture AuthorizationServer

AuthorizationServer is the foundation for implementing application and API authorization.
As a first step, we provide an implementation of the OAuth2 framework.

**This is a really early version - for feedback, bug reports, feature ideas etc., please use the issue tracker**


### Concepts

We support the following primitives:

**Applications**
Applications are containers for settings (token lifetime, key material, audience…) and scopes. Every application gets its own entry point in the URL structure, e.g. /as/app/oauth.

**Scopes**
Scopes represent permissions a client can ask for. They will be shown on consent screens, so the resource owner can grant (or deny) access. A scope defines also which client can request it.

**Clients**
A client has a client ID and a secret. A client can use exactly one OAuth2 flow to request tokens (code, implicit, resource owner credentials, client credentials). A client has a list of allowed redirect URIs for flows that require a callback.

**Access Tokens**
An access token will contain JWT standard claims like iss (issuer), aud (audience), nbf (not before), exp (expiration). In addition it will contain information about the subject (sub claim), the client that requested the token as well as the requested scopes.
