using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.IO;
using System.ComponentModel;
using System.Runtime.Serialization.Formatters.Binary;

using Andamio.Serialization;

namespace Andamio.Data.Entities
{
    /// <summary>
    /// Base class for all data entities. 
    /// </summary>
    /// <remarks>Entities are simply type safe data containers.</remarks>
    [Serializable]
    [DataContract(IsReference=true)]    
    public abstract class EntityBase : INotifyPropertyChanged, ICloneable
    {
        #region Public Events
        /// <summary>
        /// Event that fires whenever a property is changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            if (!IsSerializing)
            { IsModified = true; }

            if (PropertyChanged != null)
            { PropertyChanged(this, args); }
        }

        #endregion

        #region Public Properties
        /// <summary>
        /// Determines whether entity has been modified.
        /// </summary>
        [DataMember]
        public virtual bool IsModified { get; set; }

        /// <summary>
        /// Determines whether entity is read from database.
        /// </summary>
        [DataMember]
        public virtual bool IsReadFromDatabase  { get; set; }

        #endregion

        #region Cloning
        /// <summary>
        /// Creates a new instance of the class with the same value as instance.
        /// </summary>
        /// <returns>Returns cloned object.</returns>
        public virtual object Clone()
        {
            IFormatter formatter = new NetDataContractSerializer();
            using(Stream stream = new MemoryStream())
            {
                formatter.Serialize(stream, this);
                stream.Seek(0, SeekOrigin.Begin);                               
                return formatter.Deserialize(stream);
            }
        }

        #endregion

        #region Serialization
        /// <summary>
        /// True if entity is currently being Serialized; false otherwise.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public bool IsSerializing { get; set; }

        [OnSerializing]
        void OnSerializing(StreamingContext context)
        {
            IsSerializing = true;
            OnSerializing();
        }

        [OnSerialized]
        void OnSerialized(StreamingContext context)
        {
            IsSerializing = false;
        }

        [OnDeserializing]
        void OnDeserializing(StreamingContext context)
        {
            IsSerializing = true;
            OnDeserializing();
        }

        [OnDeserialized]
        void OnDeserialized(StreamingContext context)
        {
            IsSerializing = false;
        }

        /// <summary>
        /// Executes before entity deserialization.
        /// </summary>
        public virtual void OnDeserializing()
        {
        }

        /// <summary>
        /// Executes before entity serialization.
        /// </summary>
        public virtual void OnSerializing()
        {
        }

        #endregion 
    }
}
