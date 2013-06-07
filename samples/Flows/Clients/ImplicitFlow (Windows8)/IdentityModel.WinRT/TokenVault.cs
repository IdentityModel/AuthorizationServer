using System.Linq;
using Thinktecture.IdentityModel.Extensions;
using Windows.Data.Json;
using Windows.Security.Credentials;

namespace Thinktecture.IdentityModel.WinRT
{
    public static class TokenVault
    {
        public static void StoreToken(string identifier, TokenResponse response)
        {
            var json = new JsonObject();

            json["access_token"] = JsonValue.CreateStringValue(response.AccessToken);
            json["expires_in"] = JsonValue.CreateNumberValue(response.ExpiresIn);
            json["token_type"] = JsonValue.CreateStringValue(response.TokenType);

            var vault = new PasswordVault();
            vault.Add(new PasswordCredential(identifier, "token", json.Stringify()));
        }

        public static bool TryGetToken(string resourceName, out TokenCredential tokenCredential)
        {
            var vault = new PasswordVault();
            tokenCredential = null;

            try
            {
                var creds = vault.FindAllByResource(resourceName);
                if (creds != null)
                {
                    var credential = creds.First();
                    credential.RetrievePassword();
                    var json = JsonObject.Parse(credential.Password);

                    tokenCredential = new TokenCredential
                    {
                        AccessToken = json["access_token"].GetString(),
                        TokenType = json["token_type"].GetString(),
                    };

                    double expiresIn = json["expires_in"].GetNumber();
                    var dt = ((long)expiresIn).ToDateTimeFromEpoch();

                    tokenCredential.Expires = dt;

                    return true;
                }
            }
            catch
            { }

            return false;
        }
    }
}
