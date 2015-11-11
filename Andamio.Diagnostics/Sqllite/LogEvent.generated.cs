using System;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Andamio;
using Andamio.Configuration;
using Andamio.Data;
using Andamio.Data.Access;
using Andamio.Data.Entities;
using Andamio.Data.Transactions;
using Andamio.Data.Serialization;
using Andamio.Diagnostics;

namespace Andamio.Diagnostics.Sqllite
{
    #region Collection
	[Serializable()]
    [CollectionDataContract]
    public partial class LogEvents : EntityCollection<LogEvent, int>
    {
        #region Constructors
        public LogEvents() : base()
        {
        }

        public LogEvents(IEnumerable<LogEvent> items)
            : base(items)
        {
        }

        #endregion

        #region Conversion
        public static implicit operator LogEvents(List<LogEvent> entities)
        {
            return new LogEvents(entities);
        }

        #endregion
    }

    #endregion


    #region Entity
    [Serializable()]
    [DataContract(IsReference = true)]
    public partial class LogEventEntity : Entity<LogEvent, int>
    {
        #region Constructors
        public LogEventEntity() : base()
        {
        }

        public LogEventEntity(LogEventEntity entity)
            : base(entity)
        {
        }

        #endregion

        #region Conversion
        public static implicit operator LogEventEntity(LogEvent entity)
        {
            return new LogEventEntity(entity);
        }

        public static implicit operator LogEvent(LogEventEntity entity)
        {
            return entity.Value;
        }

        #endregion
    }

    #endregion


	[Serializable]
	[DataContract(IsReference = true)]
	public partial class LogEvent : SimpleKeyEntity<int>
	{	
        #region Constructor
        public LogEvent() : base()
        {
            Initialize();
        }

        public LogEvent(LogEvent copy) : this()
        {
			this.ID = copy.ID;
			this.Type = copy.Type;
			this.Message = copy.Message;
			this.Owner = copy.Owner;
			this.Content = copy.Content;
			this.TimeStamp = copy.TimeStamp;
        }

		#endregion
		
		#region Properties
		[DataMember]
        public LogEventType Type
        {
            get
            {
                return _Type;
            }
            set
            {
                OnTypeChanging(value);
                _Type = value;
				OnPropertyChanged(new PropertyChangedEventArgs("Type"));
                OnTypeChanged();				
            }
        }
        private LogEventType _Type;
        partial void OnTypeChanging(LogEventType value);
        partial void OnTypeChanged();
		
		[DataMember]
        public string Message
        {
            get
            {
                return _Message;
            }
            set
            {
                OnMessageChanging(value);
                _Message = value;
				OnPropertyChanged(new PropertyChangedEventArgs("Message"));
                OnMessageChanged();				
            }
        }
        private string _Message;
        partial void OnMessageChanging(string value);
        partial void OnMessageChanged();
		
		[DataMember]
        public string Owner
        {
            get
            {
                return _Owner;
            }
            set
            {
                OnOwnerChanging(value);
                _Owner = value;
				OnPropertyChanged(new PropertyChangedEventArgs("Owner"));
                OnOwnerChanged();				
            }
        }
        private string _Owner;
        partial void OnOwnerChanging(string value);
        partial void OnOwnerChanged();
		
		[DataMember]
        public byte[] Content
        {
            get
            {
                return _Content;
            }
            set
            {
                OnContentChanging(value);
                _Content = value;
				OnPropertyChanged(new PropertyChangedEventArgs("Content"));
                OnContentChanged();				
            }
        }
        private byte[] _Content;
        partial void OnContentChanging(byte[] value);
        partial void OnContentChanged();	
		
		[DataMember]
        public string TimeStamp
        {
            get
            {
                return _TimeStamp;
            }
            set
            {
                OnTimeStampChanging(value);
                _TimeStamp = value;
				OnPropertyChanged(new PropertyChangedEventArgs("TimeStamp"));
                OnTimeStampChanged();				
            }
        }
        private string _TimeStamp;
        partial void OnTimeStampChanging(string value);
        partial void OnTimeStampChanged();
		
		#endregion
		
        #region Initialization
        public override void OnDeserializing()
        {
            Initialize();
        }

		partial void OnInitialize();
		protected virtual void Initialize()
		{
		
					
			OnInitialize();
		}

        #endregion
	}
}
