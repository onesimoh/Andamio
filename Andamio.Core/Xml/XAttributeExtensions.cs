using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Andamio
{
    static public class XAttributeExtensions
    {
        #region Enum
        /// <summary></summary>
        static public EnumT Enum<EnumT>(this XAttribute attribute, EnumT defaultValue)
            where EnumT : struct
        {
            if ((attribute == null) || attribute.Value.IsNullOrBlank())
            {
                return defaultValue;
            }
            else if (!System.Enum.IsDefined(typeof(EnumT), attribute.Value))
            {
                return defaultValue;
            }
            else
            {
                return (EnumT)System.Enum.Parse(typeof(EnumT), attribute.Value);
            }
        }

        /// <summary></summary>
        static public EnumT Enum<EnumT>(this XAttribute attribute)
            where EnumT : struct
        {
            if ((attribute == null) || attribute.Value.IsNullOrBlank())
            { throw new NullReferenceException(); }

            return (EnumT)System.Enum.Parse(typeof(EnumT), attribute.Value);
        }

        public static EnumT? NullableEnum<EnumT>(this XAttribute attribute)
            where EnumT : struct
        {
            EnumT enumValue;
            if ((attribute != null) && !attribute.Value.IsNullOrBlank() && System.Enum.TryParse<EnumT>(attribute.Value, out enumValue))
            {
                return enumValue;
            }

            return null;
        }

        #endregion

        #region Guid
        /// <summary>
        /// 
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        static public Guid? NullableGuid(this XAttribute attribute)
        {
            return attribute.NullableGuid(null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        static public Guid? NullableGuid(this XAttribute attribute, Guid? defaultValue)
        {
            if (attribute == null)
            { return defaultValue; }
            return attribute.Value.IsNullOrBlank() ? defaultValue : new Guid(attribute.Value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        static public Guid Guid(this XAttribute attribute)
        {
            if (attribute == null) throw new NullReferenceException();            
            if (attribute.Value.IsNullOrBlank()) throw new NullReferenceException();            
            return new Guid(attribute.Value);
        }
        #endregion

        #region Int32
        /// <summary>
        /// 
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        static public int? NullableInt32(this XAttribute attribute)
        {
            return attribute.NullableInt32(null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        static public int? NullableInt32(this XAttribute attribute, Int32? defaultValue)
        {
            if (attribute == null)
            { return defaultValue; }
            return attribute.Value.IsNullOrBlank() ? defaultValue : System.Int32.Parse(attribute.Value);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        static public int Int32(this XAttribute attribute)
        {
            if (attribute == null) throw new NullReferenceException();            
            if (attribute.Value.IsNullOrBlank()) throw new NullReferenceException();            
            return System.Int32.Parse(attribute.Value);
        }

        #endregion

        #region DateTime
        /// <summary>
        /// 
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        static public DateTime? NullableDateTime(this XAttribute attribute)
        {
            return attribute.NullableDateTime(null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        static public DateTime? NullableDateTime(this XAttribute attribute, DateTime? defaultValue)
        {
            if (attribute == null)
            { return defaultValue; }
            return (attribute.Value.IsNullOrBlank()) ? defaultValue : System.DateTime.Parse(attribute.Value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        static public DateTime DateTime(this XAttribute attribute)
        {
            if (attribute == null) throw new NullReferenceException();
            if (attribute.Value.IsNullOrBlank()) throw new NullReferenceException();
            return System.DateTime.Parse(attribute.Value);
        }


        #endregion

        #region Boolean
        public static bool Bool(this XAttribute attribute)
        {
            if (attribute == null) throw new ArgumentNullException();
            return Boolean.Parse(attribute.Value);
        }

        public static bool OptionalBool(this XAttribute attribute, bool defaultValue = false)
        {
            bool value;
            if (attribute != null && !attribute.Value.IsNullOrBlank() && Boolean.TryParse(attribute.Value, out value))
            { return value; }
            return defaultValue;            
        }

        /// <summary></summary>
        static public bool? NullableBool(this XAttribute attribute)
        {
            bool value;
            if ((attribute != null) && !attribute.Value.IsNullOrBlank() && Boolean.TryParse(attribute.Value, out value))
            {
                return value;
            }

            return null;
        }

        #endregion

        #region Float
        /// <summary></summary>
        static public float? NullableFloat(this XAttribute attribute)
        {
            float value;
            if ((attribute != null) && !attribute.Value.IsNullOrBlank() && float.TryParse(attribute.Value, out value))
            {
                return value;
            }

            return null;
        }

        #endregion

        #region Color
        /// <summary></summary>
        public static Color? NullableColor(this XAttribute attribute)
        {
            string color;
            if ((attribute != null) && attribute.TryGetValue(out color))
            {
                return color.StartsWith("#") ? ColorTranslator.FromHtml(color) : System.Drawing.Color.FromName(color);
            }

            return null;
        }

        /// <summary></summary>
        public static Color Color(this XAttribute attribute)
        {
            string color;
            if ((attribute == null) || !attribute.TryGetValue(out color))
            { throw new ArgumentException("attribute"); }
            return color.StartsWith("#") ? ColorTranslator.FromHtml(color) : System.Drawing.Color.FromName(color);
        }

        #endregion

        #region Optional
        /// <summary>
        /// 
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        static public bool TryGetValue(this XAttribute attribute, out string value)
        {
            if (attribute == null)
            {
                value = null;
                return false;
            }
            else
            {
                value = attribute.Value;
                return true;
            }
        }

        /// <summary></summary>
        static public string Optional(this XAttribute attribute)
        {
            return attribute.Optional(null);
        }

        /// <summary></summary>
        static public string Optional(this XAttribute attribute, string defaultValue)
        {
            return ((attribute != null) && (attribute.Value != null)) ? attribute.Value : defaultValue;
        }

        static public T Optional<T>(this XAttribute attribute, T defaultValue = default(T))
            where T : struct
        {
            return ((attribute != null) && !attribute.Value.IsNullOrBlank()) ? attribute.Value.Parse<T>(defaultValue) : defaultValue;
        }

        #endregion

    }
}
