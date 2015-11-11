using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Andamio.Security
{
    public static class SecurityAuthority
    {
        public static void AssertUserRoles(params string[] roles)
        {
            UserIdentity.GetCurrentPrincipal().AssertRoles(roles);
        }
    }
}
