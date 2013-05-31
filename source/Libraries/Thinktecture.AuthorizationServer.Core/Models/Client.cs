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
        public virtual string ClientId { get; set; }
        [Required]
        public virtual string ClientSecret { get; set; }
        public virtual ClientAuthenticationMethod AuthenticationMethod { get; set; }
        [Required]
        public virtual string Name { get; set; }
        public virtual OAuthFlow Flow { get; set; }
        public virtual bool AllowRefreshToken { get; set; }

        // only relevant if Flow == Code or Implicit
        public virtual bool RequireConsent { get; set; }

        public virtual List<ClientRedirectUri> RedirectUris { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Flow == OAuthFlow.Implicit && AllowRefreshToken)
            {
                yield return new ValidationResult("Implicit Flow can not use Refresh Tokens", new[]{"Code", "AllowRefreshToken"});
            }
        }
    }
}
