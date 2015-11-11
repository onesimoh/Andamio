using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.DirectoryServices;

using Andamio;
using Andamio.Configuration;
using Andamio.Security;
using Andamio.Security.Configuration;

namespace Andamio.Security.ActiveDirectory
{
    public class ActiveDirectory
    {
        #region Constructors
        public ActiveDirectory()
        {
            SecurityAuthConfigSection configSection = ConfigHelper.LoadConfiguration<SecurityAuthConfigSection>(SecurityAuthConfigSection.ElementName);
            Entry = new DirectoryEntry(configSection.ActiveDirectoryPath);            
        }

        public ActiveDirectory(string directoryEntryPath)
        {
            if (String.IsNullOrEmpty(directoryEntryPath)) throw new ArgumentNullException("directoryEntryPath");
            Entry = new DirectoryEntry(directoryEntryPath);
        }
        
        public ActiveDirectory(DirectoryEntry directoryEntry)
        {
            if (directoryEntry == null) throw new ArgumentNullException("directoryEntry");
            Entry = directoryEntry;
        }
        #endregion

        #region DirectoryEntry
        public DirectoryEntry Entry { get; private set; }

        #endregion

        #region Users
        public UserDescriptor GetUser(string accountName)
        {
            DirectorySearcher directorySearcher = new DirectorySearcher(Entry) { Filter = String.Format("samaccountname={0}", accountName) };
            var results = directorySearcher.FindAll();
            if (!results.Any())
            { 
                throw new UserNotFoundException(accountName, String.Format("Specified User '{0}' was not found in Active Directory.", accountName)); 
            }

            return UserDescriptor.Initialize(results.First());
        }

        public UserDescriptor GetUser(DistinguishedName distinguishedName)
        {
            if (distinguishedName == null) throw new ArgumentNullException("distinguishedName");
            using (DirectoryEntry entry = new DirectoryEntry(String.Format("LDAP://{0}", distinguishedName)))
            {
                using (DirectorySearcher directorySearcher = new DirectorySearcher(entry))
                {
                    var results = directorySearcher.FindAll();
                    if (!results.Any())
                    {
                        throw new UserNotFoundException(distinguishedName.Name, String.Format("Specified User '{0}' was not found in Active Directory.", distinguishedName)); 
                    }
                    UserDescriptor user = UserDescriptor.Initialize(results.First());
                    return user;
                }
            }
        }

        #endregion

        #region Groups
        public UserDescriptor[] GetGroupMembers(string groupName)
        {
            if (groupName.IsNullOrBlank())
            { throw new ArgumentNullException("groupName"); }

            DirectorySearcher directorySearcher = new DirectorySearcher(Entry) { Filter = String.Format("CN={0}", groupName) };
            SearchResultCollection searchResultCollection = directorySearcher.FindAll();

            List<UserDescriptor> groupMembers = new List<UserDescriptor>();
            foreach (SearchResult searchResult in searchResultCollection)
            {
                foreach (var member in searchResult.Properties["member"])
                {
                    DirectoryEntry directoryEntry = new DirectoryEntry(String.Format("LDAP://{0}", member));

                    UserDescriptor groupMember = UserDescriptor.Initialize(directoryEntry.Properties);
                    groupMembers.Add(groupMember);
                }
            }

            return groupMembers.ToArray();
        }

        public GroupDescriptor GetGroup(string commonName)
        {
            GroupDescriptor group;
            if (!TryGetGroup(commonName, out group))
            {
                throw new NotFoundException(String.Format("Specified Group '{0}' was not found in Active Directory.", commonName)); 
            }
                    
            return group;
        }

        public bool TryGetGroup(string commonName, out GroupDescriptor group)
        {
            if (commonName.IsNullOrBlank()) throw new ArgumentNullException("commonName");

            using (DirectorySearcher directorySearcher = new DirectorySearcher(Entry) { Filter = String.Format("CN={0}", commonName) })
            {
                SearchResultCollection searchResult = directorySearcher.FindAll();
                if (searchResult.Any())
                {
                    group = GroupDescriptor.Initialize(searchResult.First());
                }
                else
                {
                    group = null;
                }
                return (group != null);
            }
        }

        public Groups GetGroups(OrganizationalUnit organizationalUnit)
        {
            if (organizationalUnit == null) throw new ArgumentNullException("organizationalUnit");

            Groups groups = new Groups();
            using (DirectorySearcher directorySearcher = new DirectorySearcher(Entry) { Filter = String.Format("OU={0}", organizationalUnit) })
            {
                SearchResultCollection searchResult = directorySearcher.FindAll();
                if (searchResult.Any())
                {
                    foreach (SearchResult searchResul in searchResult)
                    {
                        DirectoryEntry directoryEntry = searchResul.GetDirectoryEntry();
                        foreach (DirectoryEntry child in directoryEntry.Children)
                        {
                            GroupDescriptor group = GroupDescriptor.Initialize(child.Properties);
                            groups.Add(group);
                        }
                    }
                }                
            }

            return groups;
        }

        //public void GetGroup(DistinguishedName distinguishedName)
        //{
        //    if (distinguishedName == null) throw new ArgumentNullException("distinguishedName");

        //    using (DirectoryEntry entry = new DirectoryEntry(String.Format("LDAP://{0}", groupName)))
        //    {
        //        using (DirectorySearcher directorySearcher = new DirectorySearcher(entry))
        //        {
        //            //directorySearcher.PropertiesToLoad.AddRange(INCLUDED_RESULT_FIELDS);
        //            var searchResult = directorySearcher.FindAll();
        //        }
        //    }            
        //}

        #endregion
    }
}
