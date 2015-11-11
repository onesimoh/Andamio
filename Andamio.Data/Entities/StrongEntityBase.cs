using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace Andamio.Data.Entities
{
    /// <summary>
    /// Base class for all Strong Entity Types.
    /// </summary>
    /// <remarks>
    /// A Strong Entity is defined as one that can be uniquely identified based on its Primary Key of ID Field.
    /// </remarks>
    /// <typeparam name="T">Type of the ID field (ex. Guid)</typeparam>
    [Serializable]
    [DataContract(IsReference = true)]
    public abstract class StrongEntityBase<T> : SimpleKeyEntity<T> where T : struct
    {
        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        public StrongEntityBase() : base()
        {
            _id = GenerateUniqueIdentifier();
        }

        #endregion

        #region ID
        /// <summary>
        /// Override this method to provide the functionality to generate the Unique Id for the Entity.
        /// </summary>
        /// <remarks>
        /// You May return a New Guid value or provide your own ID generator that uniquely identifies this Entity.
        /// </remarks>
        /// <returns>Type of the ID field (ex. Guid)</returns>
        protected abstract T GenerateUniqueIdentifier();

        /// <summary>
        /// Gets or sets the unique ID for Entity.
        /// </summary>
        [DataMember]
        public override T ID
        {
            get
            {
                if (!_id.HasValue)
                { _id = GenerateUniqueIdentifier(); }
                return _id.Value;
            }
            set 
            {
                OnIDChanging(value);
                base.ID = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ID"));
                OnIDChanged();
            }
        }

        /// <summary>
        /// Executes before the ID Property is assigned.
        /// </summary>
        /// <param name="value">ID values being assigned</param>
        protected virtual void OnIDChanging(T value)
        {
        }

        /// <summary>
        /// Executes after the ID Property is assigned.
        /// </summary>
        protected virtual void OnIDChanged()
        {
        }

        #endregion
    }
}
