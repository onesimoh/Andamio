using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Andamio.Data.Entities
{
    [Serializable]
    [DataContract(IsReference = true)]
    public abstract class GuidStrongEntityBase : StrongEntityBase<Guid>
    {
        #region GenerateUniqueIdentifier
        /// <summary>
        /// Returns a new Guid value that uniquely identifies this Entity.
        /// </summary>
        protected override Guid GenerateUniqueIdentifier()
        {
            return Guid.NewGuid();
        }

        #endregion
    }
}
