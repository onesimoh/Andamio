using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.ServiceModel;

using Andamio;
using Andamio.Security.Configuration;

namespace Andamio.Security
{
    public static class UserIdentity
    {
        #region Wellknown
        public static readonly string System = "NT AUTHORITY\\SYSTEM";
        public static readonly string NetworkService = "NT AUTHORITY\\NetworkService";
        public static readonly string LocalService = "NT AUTHORITY\\LocalService";

        #endregion

        #region Identity
        /// <summary>
        /// Gets the loggin user name based on current Identity associated with the running thread via
        /// Thread.CurrentPrincipal, if none is associate it will then use current Windows User.
        /// </summary>
        /// <returns>String that contains user name value.</returns>
        public static string GetIdentityName()
        {
            IIdentity identity = GetCurrent();
            string identityName = (identity != null) ? identity.GetUserName() : Environment.UserName;
            if (identityName.Equals(UserIdentity.System) || identityName.Equals(UserIdentity.NetworkService)
                || identityName.Equals(UserIdentity.LocalService))
            {
                identityName = identityName.Replace("NT AUTHORITY\\", String.Empty);
            }
            


            return identityName;
        }

        /// <summary>
        /// Gets the current Identity associated with the running thread via
        /// Thread.CurrentPrincipal, if none is associate it will then use current Windows User.
        /// </summary>
        /// <returns>User Identity.</returns>
        public static IIdentity GetCurrent()
        {
            SecurityAuthConfigSection securityConfig = SecurityAuthConfigSection.FromConfig();
            if (securityConfig != null && !securityConfig.Impersonate.IsNullOrBlank())
            {
                return new GenericIdentity(securityConfig.Impersonate);
            }

            if (HttpContext.Current != null && HttpContext.Current.User != null)
            {
                IIdentity httpContextIndentity = HttpContext.Current.User.Identity;
                if (httpContextIndentity != null && httpContextIndentity.IsAuthenticated)
                {
                    return httpContextIndentity;
                }
            }

            IPrincipal serviceSecurityPrincipal = GetServiceSecurityPrincipal();
            if (serviceSecurityPrincipal != null)
            {
                return serviceSecurityPrincipal.Identity;
            }

            // Return logged in Windows Identity if available
            IIdentity windowsIdentity = WindowsIdentity.GetCurrent();
            if (windowsIdentity != null && windowsIdentity.IsAuthenticated)
            {
                return windowsIdentity;
            }

            // Return Current Thread Identity if available
            return Thread.CurrentPrincipal.Identity;
        }

        #endregion

        #region Principal
        public static IPrincipal GetCurrentPrincipal()
        {
            SecurityAuthConfigSection securityConfig = SecurityAuthConfigSection.FromConfig();
            if (securityConfig != null && !securityConfig.Impersonate.IsNullOrBlank())
            {
                WindowsIdentity identity = new WindowsIdentity(securityConfig.Impersonate);
                return new WindowsPrincipal(identity);
            }

            return (HttpContext.Current != null && HttpContext.Current.User != null)
                ? HttpContext.Current.User
                : Thread.CurrentPrincipal;
        }

        public static IPrincipal GetServiceSecurityPrincipal()
        {
            IPrincipal principal = null;
            try
            {
                if (ServiceSecurityContext.Current != null && OperationContext.Current.ServiceSecurityContext != null
                    && OperationContext.Current.ServiceSecurityContext.AuthorizationContext.Properties.Any())
                {
                    object principalObj;
                    if (OperationContext.Current.ServiceSecurityContext.AuthorizationContext.Properties.TryGetValue("Principal", out principalObj))
                    {
                        principal = principalObj as IPrincipal;
                    }
                }
            }
            catch { }

            return principal;
        }

        #endregion



        #region Impersonation
        public static void ImpersonateUser(UserCredentials userCredentials)
        {

            throw new NotImplementedException();
        }


        public static WindowsImpersonationContext ImpersonateWindowsIdentity()
        {
            WindowsIdentity windowsIdentity = GetCurrent() as WindowsIdentity;
            if (windowsIdentity == null)
            { throw new InvalidOperationException("Impersonation is only allowed on Windows Identity"); }

            WindowsImpersonationContext context = WindowsIdentity.Impersonate(windowsIdentity.Token);
            return context;
        }

        #endregion
    }
}
