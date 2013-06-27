using System.ComponentModel.DataAnnotations;

namespace Thinktecture.AuthorizationServer.WebHost.Areas.Admin.Models
{
    public class SymmetricKeyModel
    {
        [Required]
        public string Name { get; set; }
        public string Value { get; set; }
    }
}