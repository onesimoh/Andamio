using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Andamio.Security.ActiveDirectory
{
    public class OrganizationalUnit
    {
        #region Constructors
        private OrganizationalUnit()
        {
        }

        public OrganizationalUnit(string name)
        {
            if (name.IsNullOrBlank()) throw new ArgumentNullException("name");
            Name = name;
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
