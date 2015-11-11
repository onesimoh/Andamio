using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.DirectoryServices;
using System.Diagnostics;

using Andamio;
using Andamio.Collections;

namespace Andamio.Security.ActiveDirectory
{
    public class UserDescriptorCollection : CollectionBase<UserDescriptor>
    {
        #region Constructors
        public UserDescriptorCollection() 
            : base()
        {
        }

        public UserDescriptorCollection(IEnumerable<UserDescriptor> users)
            : base(users)
        {
        }

        #endregion

        #region Indexers
        public UserDescriptor this[string userName]
        {
            get
            {
                return this.SingleOrDefault(match => match.AccountName.Equals(userName, StringComparison.OrdinalIgnoreCase));
            }
        }

        #endregion
    }


    [DebuggerDisplay("{DisplayName} ({AccountName})")]
    public class UserDescriptor
    {
        #region Constructors
        private UserDescriptor()
        {
        }

        private UserDescriptor(string accountName)
        {
            AccountName = accountName;
        }
        #endregion

        #region Factory
        public static UserDescriptor For(string userName)
        {
            ActiveDirectory activeDirectory = new ActiveDirectory();
            UserDescriptor userDescriptor = activeDirectory.GetUser(userName);
            return userDescriptor;        
        }

        #endregion

        #region Properties
        /// <summary>
        /// The AccountName of the user (SAMAccountName); most likely, the user's login/domain username.
        /// </summary>
        public string AccountName { get; internal set; }

        /// <summary>
        /// The User's First Name. 
        /// </summary>
        public string FirstName { get; internal set; }

        /// <summary>
        /// The User's Last Name. 
        /// </summary>
        public string LastName { get; internal set; }

        /// <summary>
        /// The display name of the user; most likely, the user's full name.
        /// </summary>
        public string DisplayName { get; internal set; }

        /// <summary>
        /// The primary email address of the user.
        /// </summary>
        public string Mail { get; internal set; }

        /// <summary>
        /// The user's company.
        /// </summary>
        public string Company { get; internal set; }

        /// <summary>
        /// The user's country.
        /// </summary>
        public string Country { get; internal set; }

        /// <summary>
        /// The Location of the user.
        /// </summary>
        public string Location { get; internal set; }

        /// <summary>
        /// The Department of the user
        /// </summary>
        public string Department { get; internal set; }

        /// <summary>
        /// The Manager of the user.
        /// </summary>
        public string Manager { get; internal set; }

        /// <summary>
        /// The Title of the user.
        /// </summary>
        public string Title { get; internal set; }

        /// <summary>
        /// The department to which the user belongs.
        /// </summary>
        public string Description { get; internal set; }

        /// <summary>
        /// Phone Number.
        /// </summary>
        public string PhoneNumber { get; internal set; }

        /// <summary>
        /// Various groups to which the user belongs.
        /// </summary>
        public string[] GroupsMembership { get; internal set; }

        #endregion

        #region Initialization
        internal static UserDescriptor Initialize(ResultPropertyCollection properties)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary.AddRange(properties.ToKeyValuePairs());
            return Initialize(dictionary);
        }
        internal static UserDescriptor Initialize(SearchResult searchResult)
        {
            return Initialize(searchResult.Properties);
        }
        internal static UserDescriptor Initialize(PropertyCollection properties)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary.AddRange(properties.ToKeyValuePairs());
            return Initialize(dictionary);
        }
        internal static UserDescriptor Initialize(Dictionary<string, object> properties)
        {
            UserDescriptor userDescriptor = new UserDescriptor()
            {
                AccountName = properties.StringValue("samaccountname").ToUpper(),
                FirstName = properties.StringValue("givenname"),
                LastName = properties.StringValue("sn"),
                DisplayName = properties.StringValue("displayname"),
                Mail = properties.StringValue("mail"),
                Company = properties.StringValue("company"),
                Country = properties.StringValue("co"),
                Location = properties.StringValue("l"),
                Department = properties.StringValue("department"),                
                Title = properties.StringValue("title"),
                Description = properties.StringValue("description"),
                PhoneNumber = properties.StringValue("telephonenumber"),                
            };

            ActiveDirectory activeDirectory = new ActiveDirectory();
            DistinguishedName managerDN = properties.StringValue("manager");
            if (managerDN != null)
            {
                UserDescriptor manager = activeDirectory.GetUser(managerDN);
                if (manager != null)
                {
                    userDescriptor.Manager = manager.AccountName;
                }
            }

            /*
            if (properties.ContainsKey("memberof"))
            {
                var groups = (System.Collections.ICollection) properties["memberof"];
                foreach (var group in groups)
                {
                    activeDirectory.GetGroup(group.ToString());
                }
            }
            */

            return userDescriptor;
        }

        #endregion

        #region Comparer
        public static readonly UserDescriptorComparer Comparer = new UserDescriptorComparer();
        public class UserDescriptorComparer : IEqualityComparer<UserDescriptor>
        {
            public bool Equals(UserDescriptor left, UserDescriptor right)
            {
                return left.AccountName.Equals(right.AccountName, StringComparison.OrdinalIgnoreCase);
            }

            public int GetHashCode(UserDescriptor userDescriptor)
            {
                return userDescriptor.AccountName.GetHashCode();
            }
        }

        #endregion
    }
}
