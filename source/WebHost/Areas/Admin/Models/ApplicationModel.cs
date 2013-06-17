/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System;
using System.ComponentModel.DataAnnotations;

namespace Thinktecture.AuthorizationServer.WebHost.Areas.Admin.Models
{
    public class ApplicationModel
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public string LogoUrl { get; set; }
        [Required]
        public string Namespace { get; set; }
        [Required]
        public string Audience { get; set; }
        [Range(0, Int32.MaxValue)]
        public int TokenLifetime { get; set; }
        public bool AllowRefreshToken { get; set; }
        public bool RequireConsent { get; set; }
        public bool RememberConsentDecision { get; set; }
        public int SigningKeyID { get; set; }
        public bool Enabled { get; set; }
    }
}