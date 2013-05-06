/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System.Collections.Generic;
using System.Linq;

namespace Thinktecture.AuthorizationServer.Core.Models
{
    public class Clients : List<Client>
    {
        public Client Get(string clientId)
        {
            return (from c in this
                    where c.ClientId.Equals(clientId)
                    select c)
                   .FirstOrDefault();
        }

        public Client ValidateClient(string clientId, string clientSecret)
        {
            // todo: hashing etc

            var client = Get(clientId);
            if (client == null)
            {
                return null;
            }

            if (client.ClientSecret == clientSecret)
            {
                return client;
            }

            return null;
        }
    }
}