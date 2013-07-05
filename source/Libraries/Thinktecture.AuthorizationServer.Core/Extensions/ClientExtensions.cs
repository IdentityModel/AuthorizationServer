using System.Collections.Generic;
using System.Linq;

namespace Thinktecture.AuthorizationServer.Models
{
    public static class ClientExtensions
    {
        public static Client Get(this IEnumerable<Client> clients, string clientId)
        {
            return (from c in clients
                    where c.ClientId.Equals(clientId)
                    select c)
                   .FirstOrDefault();
        }

        public static Client ValidateClient(this IEnumerable<Client> clients, string clientId, string clientSecret)
        {
            // todo: hashing etc

            var client = clients.Get(clientId);
            if (client == null)
            {
                return null;
            }

            if (client.ValidateSharedSecret(clientSecret))
            {
                return client;
            }

            return null;
        }
    }
}
