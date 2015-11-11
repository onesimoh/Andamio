using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Reflection;
using System.Collections;
using System.Text.RegularExpressions;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Andamio
{
    public class DynamicField
    {
        #region Constructors
        internal DynamicField(JObject container, string fieldName)
        {
            if (container == null) throw new ArgumentNullException("container");
            if (fieldName.IsNullOrBlank()) throw new ArgumentNullException("fieldName");
            _Container = container;
            _FieldName = fieldName;
        }

        protected readonly JObject _Container;
        protected readonly string _FieldName;

        #endregion

        #region Optional
        public virtual DynamicField NotOptional()
        {
            JToken token;
            if (!_Container.TryGetValue(_FieldName, out token))
            {
                throw new MissingFieldException(System.String.Format("'{0}' Field Not Found!", _FieldName));
            }
            return this;
        }

        #endregion

        #region String
        public virtual string String()
        {
            return (_Container[_FieldName] != null) ? _Container[_FieldName].ToString() : null;
        }

        #endregion

        #region Bool
        public virtual bool Bool()
        {
            return String().Bool();
        }

        #endregion

        #region Nullable
        public virtual T? Null<T>() where T : struct
        {
            string value = String();
            return !value.IsNullOrBlank() ? value.Parse<T>() : (T?)null;
        }

        #endregion

        #region Enum
        public virtual E Enum<E>(E defaultValue) where E : struct
        {
            return String().ParseEnum<E>(defaultValue);
        }

        public virtual E Enum<E>() where E : struct
        {
            return String().ParseEnum<E>();
        }

        #endregion

        #region Currency
        public virtual decimal Currency(decimal defaultValue = 0)
        {
            return String().ParseCurrency(defaultValue);
        }

        #endregion

        #region Int
        public virtual int Int(int defaultValue = 0)
        {
            return (_Container[_FieldName] != null) ? _Container[_FieldName].Value<int>() : defaultValue;
        }

        #endregion

        #region Date
        public virtual DateTime Date(string format = null)
        {
            string date = String();
            if (date.IsNullOrBlank())
            {
                throw new Exception(System.String.Format("{0} is not available of contains an Empty value. A value was expected.", _FieldName));
            }
            return date.ParseDate(format);
        }

        public virtual Nullable<DateTime> NullDate(string format = null)
        {
            string date = String();
            return !date.IsNullOrBlank() ? date.ParseDate(format) : (DateTime?)null;
        }

        #endregion

        #region User
        public virtual string User()
        {
            string user = String();
            if (!user.IsNullOrBlank())
            {
                Match match = Regex.Match(user, "(?<user>[A-Za-z]+)(-S)?");
                return (match.Success) ? match.Groups["user"].Value : user;
            }
            return user;
        }

        #endregion
    }
}
