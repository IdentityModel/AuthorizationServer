namespace Thinktecture.AuthorizationServer.Core.Models
{
    public class Scope
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsCritical { get; set; }

        public Clients AllowedClients { get; set; }
    }
}
