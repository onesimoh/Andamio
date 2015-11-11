using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Andamio.Data.Entities
{
    /// <summary>
    /// Base class for all Strong Key Auditable Entities.
    /// </summary>
    /// <typeparam name="T">Type of the ID field (ex. Guid)</typeparam>
    [Serializable]
    [DataContract(IsReference = true)]
    public abstract class AuditStrongEntity<T> : StrongEntityBase<T>, IAuditable where T : struct
    {
        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        public AuditStrongEntity() : base()
        {
        }

        #endregion

        #region Audit Properties
        /// <summary>
        /// Gets or Sets user that created the entity.
        /// </summary>
        [DataMember]
        public virtual string CreatedBy { get; set; }

        /// <summary>
        /// Gets or Sets Date/Time when entity was created.
        /// </summary>
        [DataMember]
        public virtual DateTime CreatedDateTime { get; set; }

        /// <summary>
        /// Gets or Sets user that updated the entity.
        /// </summary>
        [DataMember]
        public virtual string UpdatedBy { get; set; }

        /// <summary>
        /// Gets or Sets Date/Time when entity was updated.
        /// </summary>
        [DataMember]
        public virtual DateTime UpdatedDateTime { get; set; }

        #endregion
    }
}
