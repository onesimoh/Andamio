using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace Andamio.Data.Entities
{
    #region Populate Entity Delegate
    /// <summary>
    /// Delegate Method that specifies the functionality to load the Entity Reference.
    /// </summary>
    /// <typeparam name="EntityType">The Entity Type of the reference.</typeparam>
    /// <typeparam name="EntityKey">The Entity ID Type of the reference.</typeparam>
    /// <returns></returns>
    public delegate EntityType LoadEntityMethodDelegate<EntityType, EntityKey>()
        where EntityType : SimpleKeyEntity<EntityKey>
        where EntityKey : struct, IComparable<EntityKey>;

    #endregion

    /// <summary>
    /// Represents a related entity with a multiplicity of zero or one.
    /// </summary>
    /// <typeparam name="EntityType">The Entity Type of the reference.</typeparam>
    /// <typeparam name="EntityKey">The Entity ID Type of the reference.</typeparam>
    [Serializable]
    [DataContract(IsReference=true)]
    public class Entity<EntityType, EntityKey> : INotifyPropertyChanged
        where EntityType : SimpleKeyEntity<EntityKey>
        where EntityKey : struct, IComparable<EntityKey>
    {
        #region Public Events
        /// <summary>
        /// Event is fired Entity is loaded.
        /// </summary>
        public event EventHandler Loaded;
        protected void OnLoaded(EventArgs eventArgs)
        {
            if (Loaded != null)
            { Loaded(this, eventArgs); }
        }

        /// <summary>
        /// Event is fired after Value is assigned.
        /// </summary>
        public event ItemEventHandler<EntityType> ValueAssigned;
        protected void OnValueAssigned(ItemEventArgs<EntityType> eventArgs)
        {
            if (ValueAssigned != null)
            { ValueAssigned(this, eventArgs); }
        }

        /// <summary>
        /// Event is fired after Entity Value is assigned.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            if (PropertyChanged != null)
            { PropertyChanged(this, args); }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Default Constructor
        /// </summary>
        public Entity()
        {
            IsLoadedOrAssigned = false;
        }

        /// <summary>
        /// Creates a new Entity Reference instance.
        /// </summary>
        /// <param name="entity">Entity Value wrapped to the Entity Reference.</param>
        public Entity(EntityType entity) : this()
        {
            Value = entity;
        }

        #endregion
        
        #region Value
        /// <summary>
        /// Contains a private reference to the Entity Value.
        /// </summary>
        private EntityType _value = default(EntityType);

        /// <summary>
        /// Gets or Sets the Entity Reference Value.
        /// </summary>
        [DataMember]
        public EntityType Value 
        { 
            get { return _value; } 
            set 
            {
                _value = value;                

                // Do Not Raise events if value is assigned during serialization.
                if (!IsSerializing)
                { 
                    // This always sets the Loaded Or Assigned flag to true, even, if the value is null
                    IsLoadedOrAssigned = true;                    
                    OnValueAssigned(new ItemEventArgs<EntityType>(_value)); 
                }
                OnPropertyChanged(new PropertyChangedEventArgs("Value"));
            }
        }
        
        /// <summary>
        /// True of Entity Reference is assigned; false otherwise.
        /// </summary>
        public bool HasValue
        {
            get { return Value != null; }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Assigns the Value from the specified Entity Reference.
        /// </summary>
        /// <param name="entity">The new Entity Reference containing Value to be assigned.</param>
        public void AssignEntity(Entity<EntityType, EntityKey> entity)
        {
            Value = (entity != null) ? entity.Value : null;
            IsLoadedOrAssigned = (entity != null) ? entity.IsLoadedOrAssigned : false;
        }

        /// <summary>
        /// Gets the Id value of Entity has been loaded or assigned; null otherwise.
        /// </summary>
        /// <returns></returns>
        public Nullable<EntityKey> GetIdOrNull()
        {
            if (HasValue)
            { return Value.ID; }
            else
            { return null; }
        }

        #endregion

        #region Loading
        /// <summary>
        /// True if Entity Value has been assigned; false otherwise.
        /// </summary>
        [DataMember]
        public bool IsLoadedOrAssigned { get; private set; }

        /// <summary>Load Entity Method Handler</summary>
        public LoadEntityMethodDelegate<EntityType, EntityKey> LoadEntityMethodHandler { get; set; }
        
        /// <summary>
        /// Loads the entity.
        /// </summary>
        /// <returns>True if an entity was loaded, False if Entity is Null.</returns>
        public bool Load()
        {            
            if (LoadEntityMethodHandler != null)
            {
                // LoadEntityMethodHandler should execute a method that populates the entity
                EntityType dataItem = LoadEntityMethodHandler();

                _value = dataItem;
                IsLoadedOrAssigned = true;
                OnPropertyChanged(new PropertyChangedEventArgs("Value"));            
            }

            OnLoaded(EventArgs.Empty);
            return HasValue;
        }

        /// <summary>
        /// Loads and returns the Entity.
        /// </summary>
        /// <returns></returns>
        public EntityType LoadAndGetValue()
        {
            if (Load())
            { return Value; }
            return default(EntityType);
        }

        #endregion

        #region Conversion
        /// <summary>
        /// Wraps specified Entity to Value into Entity Reference.
        /// </summary>
        /// <typeparam name="EntityType">The Entity Type of the reference.</typeparam>
        public static implicit operator Entity<EntityType, EntityKey>(EntityType entity)
        {
            return new Entity<EntityType,EntityKey>(entity);
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
