using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thinktecture.AuthorizationServer.Interfaces
{
    public interface IAuthorizationServerAdministratorsService
    {
        string[] GetAdministratorNameIDs();
    }
}
