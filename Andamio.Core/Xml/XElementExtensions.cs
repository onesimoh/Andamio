using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Text;
using System.Xml;

namespace Andamio
{
    /// <summary>
    /// Binary Xml Encoding
    /// </summary>
    public enum BinaryXmlEncoding
    {
        /// <summary>Base64</summary>
        Base64,

        /// <summary>HexString</summary>
        HexString,
    }


    static public class XElementExtensions
    {
        #region Enum Parsing
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="EnumT"></typeparam>
        /// <param name="element"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        static public EnumT ParseEnum<EnumT>(this XElement element, EnumT defaultValue)
            where EnumT : struct
        {
            if (element == null || String.IsNullOrEmpty(element.Value))
            {
                return defaultValue;
            }
            else if (!Enum.IsDefined(typeof(EnumT), element.Value))
            {
                return defaultValue;
            }
            else
            {
                return (EnumT)Enum.Parse(typeof(EnumT), element.Value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="EnumT"></typeparam>
        /// <param name="element"></param>
        /// <returns></returns>
        static public EnumT ParseEnum<EnumT>(this XElement element)
            where EnumT : struct
        {
            if (element == null || String.IsNullOrEmpty(element.Value))
            { throw new NullReferenceException(); }

            return (EnumT)Enum.Parse(typeof(EnumT), element.Value);
        }
        #endregion

        #region Guid
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        static public Guid? GetNullableGuid(this XElement element)
        {
            return element.GetNullableGuid(null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        static public Guid? GetNullableGuid(this XElement element, Guid? defaultValue)
        {
            if (element == null)
            {
                return defaultValue;
            }
            else if (String.IsNullOrEmpty(element.Value))
            {
                return defaultValue;
            }
            else
            {
                return new Guid(element.Value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        static public Guid GetGuid(this XElement element)
        {
            if (element == null)
            {
                throw new NullReferenceException();
            }
            else if (String.IsNullOrEmpty(element.Value))
            {
                throw new NullReferenceException();
            }
            else
            {
                return new Guid(element.Value);
            }
        }
        #endregion

        #region Int32
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        static public Int32? GetNullableInt32(this XElement element)
        {
            return element.GetNullableInt32(null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        static public Int32? GetNullableInt32(this XElement element, Int32? defaultValue)
        {
            if (element == null)
            {
                return defaultValue;
            }
            else if (String.IsNullOrEmpty(element.Value))
            {
                return defaultValue;
            }
            else
            {
                return Int32.Parse(element.Value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        static public Int32 GetInt32(this XElement element)
        {
            if (element == null)
            {
                throw new NullReferenceException();
            }
            else if (String.IsNullOrEmpty(element.Value))
            {
                throw new NullReferenceException();
            }
            else
            {
                return Int32.Parse(element.Value);
            }
        }
        #endregion

        #region DateTime
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        static public DateTime? GetNullableDateTime(this XElement element)
        {
            return element.GetNullableDateTime(null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        static public DateTime? GetNullableDateTime(this XElement element, DateTime? defaultValue)
        {
            if (element == null)
            {
                return defaultValue;
            }
            else if (String.IsNullOrEmpty(element.Value))
            {
                return defaultValue;
            }
            else
            {
                return DateTime.Parse(element.Value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        static public DateTime GetDateTime(this XElement element)
        {
            if (element == null)
            {
                throw new NullReferenceException();
            }
            else if (String.IsNullOrEmpty(element.Value))
            {
                throw new NullReferenceException();
            }
            else
            {
                return DateTime.Parse(element.Value);
            }
        }
        #endregion

        #region Binary
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        static public byte[] GetBinaryValue(this XElement element, BinaryXmlEncoding encoding)
        {
            if (element == null)
            {
                return new byte[0];
            }
            else if (String.IsNullOrEmpty(element.Value))
            {
                return new byte[0];
            }
            else
            {
                switch (encoding)
                {
                    case BinaryXmlEncoding.Base64:
                        return Convert.FromBase64String(element.Value);
                    case BinaryXmlEncoding.HexString:
                        return element.Value.FromHexString();
                    default:
                        throw new Exception(String.Format("Unknown Binary XML Encoding type {0}", encoding));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="encoding"></param>
        /// <param name="binaryValue"></param>
        static public void AssignBinaryValue(this XElement element, BinaryXmlEncoding encoding, byte[] binaryValue)
        {
            switch (encoding)
            {
                case BinaryXmlEncoding.Base64:
                    element.Value = Convert.ToBase64String(binaryValue);
                    break;
                case BinaryXmlEncoding.HexString:
                    element.Value = binaryValue.ToHexString();
                    break;
                default:
                    throw new Exception(String.Format("Unknown Binary XML Encoding type {0}", encoding));
            }

        }
        #endregion

        #region Optional Values
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        static public bool TryGetValue(this XElement element, out string value)
        {
            if (element == null)
            {
                value = null;
                return false;
            }
            else
            {
                value = element.Value;
                return true;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="name"></param>
        /// <param name="childElement"></param>
        /// <returns></returns>
        static public bool TryGetElement(this XElement element, XName name, out XElement childElement)
        {
            childElement = element.Element(name);
            if (childElement == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        static public string GetOptionalValue(this XElement element)
        {
            return element.GetOptionalValue(null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        static public string GetOptionalValue(this XElement element, string defaultValue)
        {
            if (element == null || String.IsNullOrEmpty(element.Value))
            {
                return defaultValue;
            }
            else
            {
                return element.Value;
            }
        }
        #endregion

        #region Write
        public static string GetInnerXmlString(this XElement element)
        {
            StringBuilder stringBuilder = new StringBuilder();
            XmlWriterSettings xmlSettings = new XmlWriterSettings() { OmitXmlDeclaration = true, Indent = true };

            using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, xmlSettings))
            {
                element.WriteTo(xmlWriter);
            }

            return stringBuilder.ToString();
        }

        #endregion
    }
}
