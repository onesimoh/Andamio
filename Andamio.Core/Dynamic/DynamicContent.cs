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
    public class DynamicContent : DynamicObject/*, IDictionary<string, object>, ICollection<KeyValuePair<string, object>>, IEnumerable<KeyValuePair<string, object>>*/
    {
        #region Constructors
        public DynamicContent()
        {
            _Container = new JObject();
        }

        public DynamicContent(object value)
        {
            _Container = (value != null) ? JObject.FromObject(value) : new JObject();
        }

        public DynamicContent(string json)
        {
            _Container = !json.IsNullOrBlank() ? JObject.Parse(json) : new JObject();
        }

        public DynamicContent(JObject json)
        {
            if (json == null) throw new ArgumentNullException("json");
            _Container = json;
        }

        public DynamicContent(XElement element)
        {
            _Container = (element != null) ? JObject.Parse(JsonConvert.SerializeXNode(element, Formatting.Indented)) : new JObject();
        }

        #endregion

        #region Indexers
        public object this[string fieldName]
        {
            get 
            {
                return _Container[fieldName]; 
            }
            set { Push(fieldName, value); }
        }

        public object this[string fieldName, int index]
        {
            get { return _Container[fieldName][index]; }
            set
            {
                if (Has(fieldName))
                {
                    if (!(_Container[fieldName] is JArray))
                    {
                        _Container[fieldName] = new JArray(_Container[fieldName]);                        
                    }
                    ((JArray) _Container[fieldName]).Insert(index, ToJson(value));
                }
            }
        }

        #endregion

        #region Fields
        public bool Has(string fieldName, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (fieldName.IsNullOrBlank()) throw new ArgumentNullException("fieldName");
            JToken token;
            return _Container.TryGetValue(fieldName, comparison, out token);
        }

        public void Push(string fieldName, object fieldValue, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (fieldName.IsNullOrBlank()) throw new ArgumentNullException("fieldName");

            JToken newValue = ToJson(fieldValue);
            if (Has(fieldName, comparison))
            {
                JToken existingValue = _Container[fieldName];
                if (existingValue is JArray)
                {
                    ((JArray)existingValue).Add(newValue);
                }
                else
                {
                    _Container[fieldName] = new JArray(existingValue, newValue);
                }
            }
            else
            {
                _Container[fieldName] = newValue;
            }
        }

        public void Push(KeyValuePair field)
        {
            if (field == null) throw new ArgumentNullException("field");
            Push(field.Key, field.Value);
        }

        public void Push(IEnumerable<KeyValuePair> fields)
        {
            if (fields == null) throw new ArgumentNullException("fields");
            fields.ForEach(f => Push(f));
        }

        public void Push<T>(IEnumerable<KeyValuePair<string, T>> fields)
        {
            if (fields == null) throw new ArgumentNullException("fields");
            fields.ForEach(f => Push(f.Key, f.Value));
        }

        public DynamicField Field(string fieldName)
        {
            if (fieldName.IsNullOrBlank()) throw new ArgumentNullException("fieldName");
            return new DynamicField(_Container, fieldName);
        }

        public DynamicField Field(string fieldName, int index)
        {
            if (fieldName.IsNullOrBlank()) throw new ArgumentNullException("fieldName");
            if (index < 0) throw new IndexOutOfRangeException("index");
            return new DynamicArrayField(_Container, fieldName, index);
        }

        public IEnumerable<T> Values<T>(string fieldName, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (fieldName.IsNullOrBlank()) throw new ArgumentNullException("fieldName");
            
            JToken token;
            if (_Container.TryGetValue(fieldName, comparison, out token))
            {
                if (token is JArray)
                {
                    return ((JArray)token).OfType<JValue>().Select(v => (T)v.Value);
                }
                else
                {
                    return new T[] { (T) token.Value<JValue>().Value };
                }
            }

            return new T[] {};
        }

        #endregion

        #region String
        public string String(string fieldName)
        {
            return Field(fieldName).String();
        }

        #endregion

        #region Bool
        public bool Bool(string fieldName)
        {
            return Field(fieldName).Bool();
        }

        #endregion

        #region Nullable
        public T? Null<T>(string fieldName) where T : struct
        {
            return Field(fieldName).Null<T>();
        }

        #endregion

        #region Enum
        public E Enum<E>(string fieldName, E defaultValue) where E : struct
        {
            return Field(fieldName).Enum<E>(defaultValue);
        }

        public E Enum<E>(string fieldName) where E : struct
        {
            return Field(fieldName).Enum<E>();
        }

        #endregion

        #region Currency
        public decimal Currency(string fieldName)
        {
            return Field(fieldName).Currency();
        }

        public decimal Currency(string fieldName, int index)
        {
            return Field(fieldName, index).Currency();
        }

        #endregion

        #region Int
        public int Int(string fieldName)
        {
            return Field(fieldName).Int();
        }

        public int Int(string fieldName, int index)
        {
            return Field(fieldName, index).Int();
        }

        #endregion

        #region Date
        public DateTime Date(string fieldName, string format = null)
        {
            return Field(fieldName).Date(format);
        }

        public DateTime Date(string fieldName, int index, string format = null)
        {
            return Field(fieldName, index).Date(format);
        }

        #endregion

        #region User
        public string User(string fieldName)
        {
            return Field(fieldName).User();
        }

        #endregion

        #region Conversion
        public static implicit operator DynamicContent(string json)
        {
            return new DynamicContent(json);
        }

        public static explicit operator JObject(DynamicContent dynamicContent)
        {
            return dynamicContent._Container;
        }

        public static implicit operator string(DynamicContent dynamicContent)
        {
            return dynamicContent.ToJson().ToString();
        }

        #endregion

        #region Factory
        public static DynamicContent From(object value)
        {
            return new DynamicContent(value);
        }

        public static DynamicContent From(string json)
        {
            return new DynamicContent(json);
        }

        public static DynamicContent From(JObject json)
        {
            return new DynamicContent(json);
        }

        public static DynamicContent From(XElement element)
        {
            return new DynamicContent(element);
        }

        #endregion

        #region Dynamic
        private JObject _Container;
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            try
            {
                result = typeof(JObject).InvokeMember(binder.Name, BindingFlags.InvokeMethod, null, _Container, args);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            try
            {
                result = _Container[binder.Name];
                return (result != null);
            }
            catch
            {
                result = null;
                return false;
            }
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            try
            {
                _Container[binder.Name] = ToJson(value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            if (indexes == null || indexes.Length != 1)
            {
                result = null;
                return false;
            }

            try
            {
                result = _Container[indexes.First()];
                return (result != null);
            }
            catch
            {
                result = null;
                return false;
            }
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            if (indexes == null || indexes.Length != 1)
            {
                return false;
            }

            try
            {
                _Container[indexes.First()] = ToJson(value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object result)
        {
            return base.TryBinaryOperation(binder, arg, out result);
        }

        public override bool TryUnaryOperation(UnaryOperationBinder binder, out object result)
        {
            return base.TryUnaryOperation(binder, out result);
        }

        #endregion

        #region Format
        public override string ToString()
        {
            return _Container.ToString();
        }

        public XElement ToXml(XName name)
        {
            if (name == null) throw new ArgumentNullException("name");
            XDocument xmlDocument = JsonConvert.DeserializeXNode(_Container.ToString(), name.LocalName);
            return xmlDocument.Root;
        }

        public JObject ToJson()
        {
            return _Container;
        }

        /// <summary>
        /// Converts the string representation of a value-type to its actual type equivalent.
        /// </summary>
        /// <param name="s">A string containing a type to convert.</param>
        /// <param name="type">Type to convert the string to.</param>
        /// <returns>Type casted value to specified Type.</returns>
        public static JToken ToJson(object value)
        {
            if (value == null) return JValue.CreateNull();

            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.String:
                    return JValue.CreateString(value.ToString());
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return new JValue(Convert.ToUInt64(value));
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return new JValue(Convert.ToInt64(value));
                case TypeCode.Decimal:
                    return new JValue(Convert.ToDecimal(value));
                case TypeCode.Double:
                    return new JValue(Convert.ToDouble(value));
                case TypeCode.Single:
                    return new JValue(Convert.ToSingle(value));
                case TypeCode.DateTime:
                    return new JValue(Convert.ToDateTime(value));
                case TypeCode.Boolean:
                    return new JValue(Convert.ToBoolean(value));
                case TypeCode.Char:
                    return new JValue(Convert.ToChar(value));
                case TypeCode.Object:
                    if (value.GetType().Equals(typeof(Guid)))
                    {
                        return new JValue(Guid.Parse(value.ToString()));
                    }
                    if (value.GetType().Equals(typeof(Uri)))
                    {
                        return new JValue(new Uri(value.ToString()));
                    }
                    if (value.GetType().IsArray || value.GetType().DerivesFromType<IEnumerable>())
                    {
                        return JArray.FromObject(value);
                    }
                    else
                    {
                        return JObject.FromObject(value);
                    }
                default:
                    return JToken.FromObject(value);
            }
        }

        public static Dictionary<string, object> Fields(JObject container)
        {
            if (container == null) throw new ArgumentNullException("container");

            Dictionary<string, object> allFields = new Dictionary<string, object>();

            foreach (KeyValuePair<string, JToken> field in container)
            {
                if (field.Value is JValue)
                {
                    allFields.Add(field.Key, ((JValue)field.Value).Value);
                }
                else if (field.Value is JArray)
                {
                    object[] value = ((JArray)field.Value).OfType<JValue>().Select(j => j.Value).ToArray();
                    allFields.Add(field.Key, value);
                }
                else if (field.Value is JObject)
                {
                    throw new NotImplementedException();
                }

            }

            return allFields;
        }

        public Dictionary<string, object> Fields()
        {
            return Fields(_Container);
        }

        /*
        public static IEnumerable<KeyValuePair<string, object>> Fields(JArray container)
        {
            if (container == null) throw new ArgumentNullException("container");

            Dictionary<string, object> allFields = new Dictionary<string, object>();

            foreach (KeyValuePair<string, JToken> field in container)
            {
                if (field.Value is JValue)
                {
                    allFields.Add(field.Key, ((JValue)field.Value).Value);
                }
                else if (field.Value is JArray)
                {

                }
                else if (field.Value is JObject)
                {
                    allFields.AddRange(Fields((JObject)field.Value));
                }
            }

            return allFields.ToArray();

        }
        */

        #endregion
    }
}
