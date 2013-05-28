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
        [Required]
        public string ClientId { get; set; }
        [Required]
        public string ClientSecret { get; set; }
        public ClientAuthenticationMethod AuthenticationMethod { get; set; }
        [Required]
        public string Name { get; set; }
        public OAuthFlow Flow { get; set; }
        public bool AllowRefreshToken { get; set; }

        // only relevant if Flow == Code or Implicit
        public bool RequireConsent { get; set; }

        public List<ClientRedirectUri> RedirectUris { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Flow == OAuthFlow.Implicit && AllowRefreshToken)
            {
                yield return new ValidationResult("Implicit Flow can not use Refresh Tokens", new[]{"Code", "AllowRefreshToken"});
            }
        }
    }
}
