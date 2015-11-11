using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Andamio.Data.Entities
{
    /// <summary>
    /// Base class for all Simple Key Auditable Entities.
    /// </summary>
    /// <typeparam name="T">The type for key values (typically Int32 or Guid).</typeparam>
    [Serializable]
    [DataContract(IsReference=true)]
    public abstract class AuditEntity<T> : SimpleKeyEntity<T>, IAuditable where T : struct
    {		    
        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        public AuditEntity() : base()
		{
		}

		#endregion		
        
        #region Audi Properties
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
