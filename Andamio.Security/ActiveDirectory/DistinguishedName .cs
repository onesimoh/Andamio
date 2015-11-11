using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Andamio.Security.ActiveDirectory
{
    public class DistinguishedNames : List<DistinguishedName>
    {
        #region Constructors
        public DistinguishedNames()
            : base()
        {
        }

        public DistinguishedNames(IEnumerable<DistinguishedName> items)
            : base(items)
        {
        }

        #endregion

        #region Users
        public UserDescriptorCollection ToUsers()
        {
            ActiveDirectory activeDirectory = new ActiveDirectory();
            UserDescriptorCollection users = new UserDescriptorCollection();
            users.AddRange(this.Select(dn => activeDirectory.GetUser(dn)).Where(u => u != null));
            return users;
        }

        #endregion
    }


    public class DistinguishedName
    {
        #region Constructors
        private DistinguishedName()
        {
        }

        public DistinguishedName(string name)
        {
            if (name.IsNullOrBlank()) throw new ArgumentNullException("name");
            Name = name;
        }

        #endregion

        #region Conversion
        public static implicit operator DistinguishedName(string distinguishedName)
        {
            return !distinguishedName.IsNullOrBlank() ? new DistinguishedName(distinguishedName) : null;
        }

        #endregion

        #region Properties
        public string Name { get; private set; }

        #endregion

        #region Overrides
        public override string ToString()
        {
            return Name;
        }

        #endregion
    }
}
