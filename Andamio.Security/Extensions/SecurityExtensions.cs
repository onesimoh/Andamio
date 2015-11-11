using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;

using Andamio;
using Andamio.Security.Configuration;

namespace Andamio.Security
{
    public static class SecurityExtensions
    {
        #region Identity
        public static string GetUserName(this IPrincipal principal)
        {
            return (principal != null) ? principal.Identity.GetUserName() : String.Empty;
        }

        public static string GetDomain(this IPrincipal principal)
        {
            string domain = null;
            if (principal != null && principal.Identity != null && !principal.Identity.Name.IsNullOrBlank())
            {
                string userName = principal.Identity.Name;
                int pos = userName.IndexOf("\\");
                domain = (pos > -1) ? userName.Substring(0, pos + 1) : String.Empty;
            }
            return domain;
        }

        public static string GetUserName(this IIdentity identity)
        {
            string userName = null;
            if (identity != null && !identity.Name.IsNullOrBlank())
            {
                userName = identity.Name;
                int pos = userName.IndexOf("\\");
                if (pos > -1)
                {
                    userName = userName.Substring(pos + 1, userName.Length - (pos + 1));
                }
            }
            return userName;
        }

        #endregion

        #region Membership
        public static string[] GetGroups(this WindowsIdentity windowsIdentity)
        {
            return windowsIdentity.Groups.Select(g => g.Translate(typeof(System.Security.Principal.NTAccount)).ToString())
                .ToArray();
        }

        #endregion

        #region Role Based Security
        public static bool VerifyRoles(this IPrincipal principal, IEnumerable<string> roles)
        {
            if (roles == null || !roles.Any())
            { return true; }

            EnvironmentConfigSection environmentConfig = EnvironmentConfigSection.FromConfig();
            if (environmentConfig.Context.IsRelease())
            {
                foreach (string role in roles)
                {
                    string r = role;
                    if (!role.StartsWith("EFGZ", StringComparison.OrdinalIgnoreCase))
                    { r = String.Format(@"EFGZ\{0}", role); }
                    if (principal.IsInRole(r))
                    { return true; }
                }
                return false;
            }
            return true;
        }

        public static bool VerifyRoles(this IPrincipal principal, params string[] roles)
        {
            return principal.VerifyRoles(new List<string>(roles));
        }

        public static void AssertRole(this IPrincipal principal, string role)
        {
            if (role.IsNullOrBlank())
            { return; }

            if (!principal.VerifyRoles(role))
            { 
                throw new UserNotAuthorizedException(UserIdentity.GetIdentityName()
                    , String.Format("'{0}' Not Authorized under Role '{1}'.", UserIdentity.GetIdentityName(), role)); 
            }

            return;
        }

        public static void AssertRoles(this IPrincipal principal, params string[] roles)
        {
            principal.AssertRoles(new List<string>(roles));
        }

        public static void AssertRoles(this IPrincipal principal, IEnumerable<string> roles)
        {
            if (roles == null || !roles.Any())
            { return; }

            if (!principal.VerifyRoles(roles))
            {
                throw new UserNotAuthorizedException(UserIdentity.GetIdentityName()
                    , String.Format("'{0}' Not Authorized under Roles: '{1}'.", UserIdentity.GetIdentityName(), roles.JoinStrings(", "))); 
            }

            return;            
        }

        public static void AssertAllRoles(this IPrincipal principal, IEnumerable<string> roles)
        {
            if (roles == null || !roles.Any())
            { return; }

            EnvironmentConfigSection environmentConfig = EnvironmentConfigSection.FromConfig();
            if (environmentConfig.Context.IsRelease())
            {
                foreach (string role in roles)
                {
                    string r = role;
                    if (!role.StartsWith("EFGZ", StringComparison.OrdinalIgnoreCase))
                    { r = String.Format(@"EFGZ\{0}", role); }
                    if (!principal.IsInRole(r))
                    { 
                        throw new UserNotAuthorizedException(UserIdentity.GetIdentityName()
                            , String.Format("'{0}' Not Authorized under Role '{1}'.", UserIdentity.GetIdentityName(), role)); 
                    }
                }
            }
            
            return; 
        }

        #endregion
    }
}
