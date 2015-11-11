using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Andamio.Data.Entities
{
    /// <summary>
    /// Interface for all Auditable entities.
    /// </summary>
    public interface IAuditable
    {
        /// <summary>
        /// Gets or Sets user that created the entity.
        /// </summary>
        string CreatedBy { get; set; }

        /// <summary>
        /// Gets or Sets Date/Time when entity was created.
        /// </summary>
        DateTime CreatedDateTime { get; set; }

        /// <summary>
        /// Gets or Sets user that updated the entity.
        /// </summary>
        string UpdatedBy { get; set; }

        /// <summary>
        /// Gets or Sets Date/Time when entity was updated.
        /// </summary>
        DateTime UpdatedDateTime { get; set; }
    }
}
