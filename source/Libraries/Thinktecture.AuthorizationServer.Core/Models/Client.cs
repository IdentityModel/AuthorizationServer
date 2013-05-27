/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Thinktecture.AuthorizationServer.Models
{
    public class Client : IValidatableObject
    {
        [Key]
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public ClientAuthenticationMethod AuthenticationMethod { get; set; }
        public string Name { get; set; }
        public OAuthFlow Flow { get; set; }
        public bool AllowRefreshToken { get; set; }
        public bool RequireConsent { get; set; }

        public List<RedirectUri> RedirectUris { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Flow != OAuthFlow.Code && AllowRefreshToken)
            {
                yield return new ValidationResult("Only CodeFlow can use Refresh Tokens", new[]{"Code", "AllowRefreshToken"});
            }
        }
    }
}
