using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices;
using System.Collections;

using Andamio;
using Andamio.Collections;

namespace Andamio.Security.ActiveDirectory
{
    public class Groups : CollectionBase<GroupDescriptor>
    {
        #region Constructors
        public Groups() : base()
        {
        }

        public Groups(IEnumerable<GroupDescriptor> groups) : base(groups)
        {
        }

        #endregion
    }


    public class GroupDescriptor
    {
        #region Constructors
        public GroupDescriptor()
        {
            Children = new Groups();
            Children.Loaded += OnChildrenLoaded;
            Children.ItemsInserted += OnChildrenInserted;

            Members = new DistinguishedNames();
        }

        #endregion

        #region Factory
        public static GroupDescriptor For(string commonName)
        {
            ActiveDirectory activeDirectory = new ActiveDirectory();
            GroupDescriptor groupDescriptor = activeDirectory.GetGroup(commonName);
            return groupDescriptor;
        }

        public static bool TryFor(string commonName, out GroupDescriptor group)
        {
            ActiveDirectory activeDirectory = new ActiveDirectory();
            return activeDirectory.TryGetGroup(commonName, out group);
        }

        #endregion

        #region Initialization
        internal static GroupDescriptor Initialize(ResultPropertyCollection properties)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary.AddRange(properties.ToKeyValuePairs());
            return Initialize(dictionary);
        }
        internal static GroupDescriptor Initialize(SearchResult searchResult)
        {
            return Initialize(searchResult.Properties);
        }
        internal static GroupDescriptor Initialize(PropertyCollection properties)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary.AddRange(properties.ToKeyValuePairs());
            return Initialize(dictionary);
        }
        internal static GroupDescriptor Initialize(Dictionary<string, object> properties)
        {
            GroupDescriptor groupDescriptor = new GroupDescriptor()
            {
                Name = properties.StringValue("name").ToUpper(),
                DistinguishedName = properties.StringValue("distinguishedname").ToUpper(),
                ObjectGuid = properties.GetValue<byte[]>("objectguid"),
                ObjectsId = properties.GetValue<byte[]>("objectsid"),
                Category = properties.StringValue("objectcategory").ToUpper(),
                Created = properties.Parse<DateTime>("whencreated"),
                Changed = properties.Parse<DateTime>("whenchanged"),
            };

            if (properties.ContainsKey("member"))
            {
                object memberProperty = properties["member"];
                if (memberProperty is ICollection)
                {
                    ICollection members = (ICollection) memberProperty;
                    if (members.Count > 0)
                    {
                        foreach (var member in members)
                        {
                            groupDescriptor.Members.Add(member.ToString());                     
                        }
                    }
                }
                else
                {
                    groupDescriptor.Members.Add(memberProperty.ToString());
                }
            }

            return groupDescriptor;
        }

        #endregion

        #region Properties
        public string Name { get; private set; }
        public string DistinguishedName { get; private set; }
        public byte[] ObjectGuid { get; private set; }
        public byte[] ObjectsId { get; private set; }
        public string Category { get; private set; } 
        public DateTime Created { get; private set; }
        public DateTime Changed { get; private set; }
        public DistinguishedNames Members { get; private set; }

        #endregion

        #region Children
        public Groups Children { get; private set; }
        void OnChildrenLoaded(object sender, EventArgs e)
        {
            
        }

        void OnChildrenInserted(object sender, ItemEventArgs<IEnumerable<GroupDescriptor>> e)
        {
            var groups = e.Item;

        }

        #endregion
    }
}
