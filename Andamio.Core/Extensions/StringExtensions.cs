using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Web;

namespace Andamio
{
    /// <summary>
    /// Provides Extensions Methods for String manipulation.
    /// </summary>
    public static class StringExtensions
    {
        #region Truncate
        public static readonly string ParenthesisEllipsis = String.Format("({0})", (char)0x2026, 1);
        public static readonly string Ellipsis = new String((char)0x2026, 1);

        /// <summary>
        /// Truncates a string.
        /// </summary>
        /// <param name="value">Value to truncate.</param>
        /// <param name="maxLength">Max amount of characters allowed.</param>
        /// <returns>Returns truncated string followed by '...'.</returns>
        public static string Truncate(this string value, int maxLength)
        {
            string retVal = String.Empty;            
            if( !String.IsNullOrEmpty(value))
            {
                retVal = (value.Length > maxLength) ? value.Substring(0, maxLength) + Ellipsis : value;
            }
            return retVal;
        }

        #endregion

        #region Concatenate
        /// <summary>
        /// This is a private method used as a converter function to convert an Int to a String.
        /// </summary>
        /// <typeparam name="T">The type of object to concatenate. T must be of type value.</typeparam>
        /// <param name="values">An array of values.</param>
        /// <returns>A comma delimited string of the array values.</returns>                
        public static string Concatenate<T>(this IEnumerable<T> values) where T : struct
        {
            return values.Join<T>(",");
        }

        /// <summary>
        /// Concatenates a specified separator character between each element of a specified 
        /// Generic Value Type array, yielding a single concatenated string.        
        /// </summary>
        /// <typeparam name="T">The type of object to concatenate. T must be of type value.</typeparam>
        /// <param name="values">An array of values.</param>
        /// <param name="separator">Separator Character</param>
        /// <returns>A string consisting of the array elements combined with the character separator.</returns>                
        public static string Concatenate<T>(this IEnumerable<T> values, char separator) where T : struct
        {
            return values.Join<T>(separator);
        }

        /// <summary>
        /// Concatenates a specified separator String between each element of a specified 
        /// Generic Value Type array, yielding a single concatenated string. 
        /// </summary>
        /// <typeparam name="T">The type of object to concatenate. T must be of type value.</typeparam>
        /// <param name="values">An array of values.</param>
        /// <param name="separator">String separator.</param>
        /// <returns>A string consisting of the array elements combined with the string separator.</returns>
        public static string Concatenate<T>(this IEnumerable<T> values, string separator) where T : struct
        {
            return values.Join<T>(separator);            
        }

        public static string Concatenate(this IEnumerable<string> values, string separator)
        {
            return String.Join(separator, values.ToArray());
        }
        public static string Concatenate(this IEnumerable<string> values)
        {
            return String.Join(", ", values.ToArray());
        }

        #endregion

        #region Parsing
        /// <summary>
        /// Converts the string representation of a value-type to its actual type equivalent.
        /// </summary>
        /// <typeparam name="T">Type to convert the string to.</typeparam>
        /// <param name="s">A string containing a type to convert.</param>
        /// <param name="result">If the conversion succeeded then this value contains the converted string.</param>
        /// <returns>true if string was converted successfully; otherwise, false.</returns>
        public static bool TryParse<T>(this string s, out T result) where T : struct
        {
            result = default(T);

            object objResult;
            if (s.TryParse(typeof(T), out objResult))
            {
                result = (T) objResult;
                return true;
            }

            return false;
        }


        /// <summary>
        /// Converts the string representation of a value-type to its actual type equivalent, if conversion fails,
        /// the specified default value is returned instead.
        /// </summary>
        /// <typeparam name="T">Type to convert the string to.</typeparam>
        /// <param name="s">A string containing a type to convert.</param>
        /// <param name="defaultValue">A default value to use in case conversion fails.</param>
        /// <returns>If the conversion succeeded the coverted value is returned; otherwise, the default value is returned.</returns>
        public static T Parse<T>(this string s, T defaultValue = default(T)) where T : struct
        {
            T result;
            if (!TryParse<T>(s, out result))
            {
                result = defaultValue;
            }
            return result;
        }

        /// <summary>
        /// Converts the string representation of a value-type to its actual type equivalent.
        /// </summary>
        /// <typeparam name="T">Type to convert the string to.</typeparam>
        /// <param name="s">A string that containing a type to convert.</param>
        /// <returns>If the conversion succeeded the coverted value is returned; null otherwise.</returns>
        public static T? ParseNullable<T>(this string s) where T : struct
        {
            T? result = null;
            T value;
            if (TryParse<T>(s, out value))
            {
                result = value;
            }
            return result;
        }


        /// <summary>
        /// Converts the specified string representation of a date and time to its DateTime equivalent using the specified culture-specific format information and formatting style. 
        /// </summary>
        /// <param name="s">A string containing a type to convert.</param>
        /// <param name="provider">An IFormatProvider that supplies formatting information about the string.</param>
        /// <param name="culture">The culture specific format.</param>
        /// <param name="result">If the conversion succeeded, contains the DateTime value equivalent to the date and time contained in the string; DateTime.MinValue if the conversion failed.</param>
        /// <returns>true if string was converted successfully; otherwise, false.</returns>
        public static bool TryParseDateTime(this string s, IFormatProvider provider, string culture, out DateTime result)
        {
            if (provider == null)
            { throw new ArgumentNullException("provider"); }

            CultureInfo ci = !String.IsNullOrEmpty(culture) ? new CultureInfo(culture) : CultureInfo.CurrentCulture;
            string formattedString = String.Format(provider, "{0}", s);

            if (!DateTime.TryParse(formattedString, ci, System.Globalization.DateTimeStyles.None, out result))
            {
                result = DateTime.MinValue;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Converts the string representation of a value-type to its actual type equivalent.
        /// </summary>
        /// <param name="s">A string containing a type to convert.</param>
        /// <param name="type">Type to convert the string to.</param>
        /// <param name="result">If the conversion succeeded then this value contains the converted string.</param>
        /// <returns>true if string was converted successfully; otherwise, false.</returns>
        public static bool TryParse(this string s, Type type, out object result)
        {
            result = null;
            try
            {
                result = s.Parse(type);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Converts the string representation of a value-type to its actual type equivalent.
        /// </summary>
        /// <param name="s">A string containing a type to convert.</param>
        /// <param name="type">Type to convert the string to.</param>
        /// <returns>Type casted value to specified Type.</returns>
        public static object Parse(this string s, Type type)
        {
            if (s == null) return null;

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.String:
                    return s;
                case TypeCode.Byte:
                    return Byte.Parse(s);
                case TypeCode.SByte:
                    return SByte.Parse(s);
                case TypeCode.UInt16:
                    return UInt16.Parse(s);
                case TypeCode.UInt32:
                    return UInt32.Parse(s);
                case TypeCode.UInt64:
                    return UInt64.Parse(s);
                case TypeCode.Int16:
                    return Int16.Parse(s);
                case TypeCode.Int32:
                    return Int32.Parse(s);
                case TypeCode.Int64:
                    return Int64.Parse(s);
                case TypeCode.Decimal:
                    return Decimal.Parse(s);
                case TypeCode.Double:
                    return Double.Parse(s);
                case TypeCode.Single:
                    return Single.Parse(s);
                case TypeCode.DateTime:
                    return DateTime.Parse(s);
                case TypeCode.Boolean:
                    return Boolean.Parse(s);
                case TypeCode.Object:
                    if (type.Equals(typeof(Guid)))
                    {
                        return Guid.Parse(s);
                    }
                    else if (type.IsNullableType())
                    {
                        return s.Parse(Nullable.GetUnderlyingType(type));
                    }
                    else
                    {
                        throw new NotSupportedException(String.Format("Parsing operation Not Supportted for Type '{0}'.", type));
                    }
                default:
                    return Convert.ChangeType(s, type);
            }
        }

        public static decimal ParseCurrency(this string s, decimal defaultValue = 0)
        {
            decimal value = !s.IsNullOrBlank() ? Decimal.Parse(s, NumberStyles.Number | NumberStyles.AllowCurrencySymbol) : defaultValue;
            return value;
        }

        public static Guid[] ParseGuids(this IEnumerable<String> values)
        {
            List<Guid> guidValues = new List<Guid>();
            if (values != null && values.Any())
            {
                foreach (string stringGuidValue in values)
                {
                    Guid guidValue;
                    if (Guid.TryParse(stringGuidValue, out guidValue))
                    {
                        guidValues.Add(guidValue);
                    }
                }
            }

            return guidValues.ToArray();
        }

        public static object ParseNumeric(this string numericValue
            , Type numericFormat
            , NumberStyles numberStyles = NumberStyles.Number | NumberStyles.Currency | NumberStyles.AllowCurrencySymbol
            , IFormatProvider formatProvider = null)
        {
            if (numericFormat == null)
            { throw new ArgumentNullException("numericFormat"); }

            switch (Type.GetTypeCode(numericFormat))
            {
                case TypeCode.Byte:
                    return Byte.Parse(numericValue, numberStyles, formatProvider);
                case TypeCode.SByte:
                    return SByte.Parse(numericValue, numberStyles, formatProvider);
                case TypeCode.UInt16:
                    return UInt16.Parse(numericValue, numberStyles, formatProvider);
                case TypeCode.UInt32:
                    return UInt32.Parse(numericValue, numberStyles, formatProvider);
                case TypeCode.UInt64:
                    return UInt64.Parse(numericValue, numberStyles, formatProvider);
                case TypeCode.Int16:
                    return Int16.Parse(numericValue, numberStyles, formatProvider);
                case TypeCode.Int32:
                    return Int32.Parse(numericValue, numberStyles, formatProvider);
                case TypeCode.Int64:
                    return Int64.Parse(numericValue, numberStyles, formatProvider);
                case TypeCode.Decimal:
                    return Decimal.Parse(numericValue, numberStyles, formatProvider);
                case TypeCode.Double:
                    return Double.Parse(numericValue, numberStyles, formatProvider);
                case TypeCode.Single:
                    return Single.Parse(numericValue, numberStyles, formatProvider);
                case TypeCode.Object:
                    Type underlyingType = Nullable.GetUnderlyingType(numericFormat);
                    return numericValue.ParseNumeric(underlyingType, numberStyles, formatProvider);
                default:
                    throw new InvalidOperationException(String.Format("Specified Type: '{0}' Is Not Numeric.", numericFormat));
            }
        }

        public static T ParseNumeric<T>(this string numericValue) where T : struct
        {
            return (T)numericValue.ParseNumeric(typeof(T));
        }

        public static bool Bool(this string value)
        {
            if (value.IsNullOrBlank())
            {
                return false;
            }

            bool result;
            if (Boolean.TryParse(value, out result))
            {
                return result;
            }
            if (value.EqualsAny("YES", "TRUE", "1"))
            {
                return true;
            }
            if (value.EqualsAny("NO", "FALSE", "0"))
            {
                return false;
            }

            throw new FormatException(String.Format("Specified value '{0}' does not conform to any of the following: True, False, 1, 0, YES, NO", value));
        }

        #endregion

        #region Break
        /// <summary>
        /// Divides a string in equal size chunks specified by the provided length.
        /// </summary>
        /// <param name="s">The string to split in equal size chunks.</param>
        /// <param name="count">The amount of items to return.</param>
        /// <param name="length">The length in characters of the chunks.</param>
        /// <returns>Returns an Array containing the individual chunks once the string is divided.</returns>        
        public static string[] Break(this string s, int count, int length)
        {
            return s.Split(count, length);
        }

        #endregion

        #region Byte Conversion
        /// <summary>
        /// Converts a string to a sequence of bytes.
        /// </summary>
        /// <param name="s">The string to convert to byte array.</param>
        /// <returns>Byte Array.</returns>
        public static byte[] ToByteArray(this string s)
        {
            return s.CovertToByteArray();
        }

        /// <summary>
        /// Converts a string to a sequence of bytes.
        /// </summary>
        /// <param name="s">The string to convert to byte array.</param>
        /// <returns>Byte Array.</returns>
        public static Byte[] CovertToByteArray(this string s)
        {
            if (!String.IsNullOrEmpty(s))
            {
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
                return encoding.GetBytes(s);
            }

            return null;
        }

        static public string ToHexString(this byte[] byteArray)
        {
            StringBuilder retVal = new StringBuilder();
            foreach (byte curByte in byteArray)
            {
                retVal.Append(curByte.ToString("X2"));
            }
            return retVal.ToString();
        }

        static public byte[] FromHexString(this string hexString)
        {
            List<byte> bytes = new List<byte>();
            int index = 0;
            while (index + 2 <= hexString.Length)
            {
                bytes.Add(Byte.Parse(hexString.Substring(index, 2), NumberStyles.HexNumber));
                index += 2;
            }
            return bytes.ToArray();
        }

        #endregion

        #region IsNullOrBlank
        /// <summary>
        /// Test the specified string, if null or empty, or when trimmed its length equals zero the returns true; otherwise false.
        /// </summary>
        /// <remarks>
        /// This method is very similar to String.IsNullOrEmpty with the addition of trimming the string value to verify it is not blank.
        /// </remarks>
        /// <param name="value">The string value to test.</param>
        /// <returns>Returns true if specified string value is null or empty, or when trimmed its length equals zero; otherwise false.</returns>
        public static bool IsNullOrBlank(this string value)
        {
            if (String.IsNullOrEmpty(value) || value.Trim().Length == 0)
            { return true; }
            return false;
        }

        #endregion

        #region Contains
        public static bool Contains(this string text, string value, StringComparison comparison)
        {
            if (comparison == StringComparison.OrdinalIgnoreCase)
            {
                return text.ToLower().Contains(value.ToLower());
            }
            else
            {
                return text.Contains(value);
            }
        }

        public static bool ContainsAll(this string text, string[] values, StringComparison comparison)
        {
            return values.All(value => text.Contains(value, comparison));
        }

        public static bool ContainsAny(this string text, string[] values, StringComparison comparison)
        {
            return values.Any(value => text.Contains(value, comparison));
        }

        public static bool EqualsToAny(this string text, StringComparison comparison = StringComparison.OrdinalIgnoreCase, params string[] values)
        {
            return values.Any(v => v.Equals(text, comparison));
        }

        #endregion

        #region Equals
        public static bool EqualsAny(this string text, params string[] values)
        {
            foreach (string value in values)
            {
                if (text.Equals(value, StringComparison.OrdinalIgnoreCase))
                { return true; }
            }
            return false;
        }

        #endregion

        #region Strip Out
        /// <summary>
        /// Removes any instances of new line character '\n\r' or '\n'.
        /// </summary>
        /// <param name="value">The string to strip out of new line characters.</param>
        /// <returns>A new string.</returns>
        public static string StripOutLineFeed(this string value)
        {
            return value.StripOutLineFeed(String.Empty);
        }

        /// <summary>
        /// Removes any instances of new line character '\n\r' or '\n' and replaces them with provided value.
        /// </summary>
        /// <param name="value">The string to strip out of new line characters.</param>
        /// <param name="replace">String to replace line feed with.</param>
        /// <returns>A new string.</returns>
        public static string StripOutLineFeed(this string value, string replace)
        {
            string retVal = String.Empty;
            if (!IsNullOrBlank(value))
            {
                retVal = value.Replace(Environment.NewLine, replace);
                retVal = retVal.Replace('\n'.ToString(), replace);
            }
            return retVal;
        }

        /// <summary>
        /// Removes any instances of White Space in the string.
        /// </summary>
        /// <param name="value">The string to remove the spaces from.</param>
        /// <returns>A new String.</returns>
        public static string StripOutSpaces(this string value)
        {
            return value.Replace(" ", String.Empty)
                .Replace("\t", String.Empty)
                .Replace("\n", String.Empty)
                .Replace("\r", String.Empty);
        }

        /// <summary>
        /// Remove bounding quotes on a token if present
        /// </summary>
        /// <param name="token">Token to unquote.</param>
        /// <returns>Unquoted token.</returns>
        public static string Unquote(this string token)
        {
            if (!token.IsNullOrBlank() 
                && token.StartsWith("\"", StringComparison.Ordinal) 
                && token.EndsWith("\"", StringComparison.Ordinal) 
                && (token.Length > 1))
            {
                return token.Substring(1, token.Length - 2);
            }

            return token;
        }

        #endregion

        #region Join
        /// <summary>
        /// This is a private method used as a converter function
        /// to convert an Int to a String.
        /// </summary>
        /// <typeparam name="T">The type of object to concatenate. T must be of type value.</typeparam>
        /// <param name="values">An array of values.</param>
        /// <returns>A comma delimited string of the array values.</returns>                
        public static string Join<T>(this IEnumerable<T> values) where T : struct
        {
            return values.Join(',');        
        }

        /// <summary>
        /// Concatenates a specified separator character between each element of a specified 
        /// Generic Value Type array, yielding a single concatenated string.        
        /// </summary>
        /// <typeparam name="T">The type of object to concatenate. T must be of type value.</typeparam>
        /// <param name="values">An array of values.</param>
        /// <param name="separator">Separator Character</param>
        /// <returns>A string consisting of the array elements combined with the character separator.</returns>                
        public static string Join<T>(this IEnumerable<T> values, char separator) where T : struct
        {
            return values.Join<T>(Convert.ToString(separator));
        }

        /// <summary>
        /// Concatenates a specified separator String between each element of a specified 
        /// Generic Value Type array, yielding a single concatenated string. 
        /// </summary>
        /// <typeparam name="T">The type of object to concatenate. T must be of type value.</typeparam>
        /// <param name="values">An array of values.</param>
        /// <param name="separator">String separator.</param>
        /// <returns>A string consisting of the array elements combined with the string separator.</returns>
        public static string Join<T>(this IEnumerable<T> values, string separator) where T : struct
        {
            if (separator == null)
            {  throw new ArgumentNullException("separator"); }

            if (values != null)
            {
                return String.Join(separator, values.Select(v => v.ToString()).ToArray());
            }
            else
            {
                return null;
            }            
        }

        /// <summary>
        /// Concatenates a specified separator String between each element of a specified String array, yielding a single concatenated string. 
        /// </summary>
        /// <param name="separator">A delimiter that separates each element of the string array in the resulting string.</param>
        /// <param name="values">The values to concatenate.</param>
        /// <param name="ignoreNullOrEmpty">if true then aray elements that are null or empty are excluded.</param>
        /// <returns>A string consisting of the array elements combined with the string separator.</returns>
        public static string JoinStrings(this IEnumerable<string> values, string separator, bool ignoreNullOrEmpty = true)
        {
            if (ignoreNullOrEmpty)
            {
                return String.Join(separator, values.Where(v => !String.IsNullOrEmpty(v)));
            }
            else
            {
                return String.Join(separator, values);
            }
        }

        #endregion

        #region Split
        /// <summary>
        /// Returns a string array that contains the substrings in this string that are delimited by elements of a specified character array.
        /// </summary>
        /// <param name="s">The string to split in equal size chunks.</param>
        /// <param name="separator">An array that delimit the substrings in this string.</param>
        /// <returns>An array whose elements contain the substrings in this string that are delimited the separator.</returns>
        public static string[] Split(this string s, char separator)
        {
            return s.Split(new char[] { separator }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Returns a string array that contains the substrings in this string that are delimited by elements of a specified character array.
        /// </summary>
        /// <param name="s">The string to split in equal size chunks.</param>
        /// <param name="separator">An array that delimit the substrings in this string.</param>
        /// <returns>An array whose elements contain the substrings in this string that are delimited the separator.</returns>
        public static string[] Split(this string s, string separator)
        {
            return s.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Returns a string array that contains the substrings in this string that are delimited by elements of a specified character array.
        /// </summary>
        /// <param name="s">The string to split in equal size chunks.</param>
        /// <returns>An array whose elements contain the substrings in this string that are delimited the separator.</returns>
        public static string[] Array(this string s)
        {
            return s.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                .Where(value => !value.IsNullOrBlank()).Select(value => value.Trim()).ToArray();
        }

        /// <summary>
        /// Divides a string in equal size chunks specified by the provided length.
        /// </summary>
        /// <param name="s">The string to split in equal size chunks.</param>
        /// <param name="count">The amount of items to return.</param>
        /// <param name="length">The length in characters of the chunks.</param>
        /// <returns>Returns an Array containing the individual chunks once the string is divided.</returns>        
        public static string[] Split(this string s, int count, int length)
        {
            string stringValue = s.Replace(Environment.NewLine, " ");

            string[] arr = new string[count];

            int pos = 0;

            for (int i = 0; i < count; i++)
            {
                string stringValueTemp = String.Empty;

                if (pos < stringValue.Length)
                {
                    int remainingLength = stringValue.Length - pos;
                    int l = (remainingLength > length) ? length : remainingLength;

                    if (i == count - 1)
                        stringValueTemp = stringValue.Substring(pos);
                    else
                        stringValueTemp = stringValue.Substring(pos, l);
                }

                arr[i] = stringValueTemp;
                pos += length;
            }

            return arr;
        }


        #endregion

        #region Validation
        /// <summary>
        /// Regular expression, which is used to validate an E-Mail address.
        /// </summary>
        public const string MatchEmailPattern = @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
           + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
           + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
           + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";
        public static readonly System.Text.RegularExpressions.Regex EmailRegex = new System.Text.RegularExpressions.Regex(MatchEmailPattern);

        /// <summary>
        /// Determines whether a value is a properly formatted email address.
        /// </summary>
        /// <param name="value">The value to verify.</param>
        /// <returns>True if the value parameter is a valid email address</returns>
        public static bool ValidateEmail(this string value)
        {
            if (value.IsNullOrBlank())
            { return false; }
            return EmailRegex.IsMatch(value);
        }

        /// <summary>
        /// Determines whether a value is a properly formatted email address.
        /// </summary>
        /// <param name="value">The value to verify.</param>
        public static void AssertValidEmail(this string value)
        {
            if (!value.ValidateEmail())
            { throw new ApplicationException(String.Format("Invalid Email Address '{0}'.", value)); }
        }

        #endregion

        #region Xml
        public static string EncodeXml(this string text)
        {
            if (text == null)
            { return String.Empty; }
            return HttpUtility.HtmlEncode(text);
        }
        #endregion

        #region Formatting
        public static string FormatString<T>(this T? value, string format)
            where T : struct
        {
            return (value.HasValue) ? String.Format(format, value) : String.Empty;
        }

        public static string FormatCurrency(this decimal amount)
        {
            return amount.ToString("C"); 
        }

        public static string FormatCurrency(this decimal? amount)
        {
            return amount.HasValue ? amount.Value.ToString("C") : String.Empty;
        }

        public static string FormatPercentage(this decimal? percentage)
        {
            return percentage.HasValue ? percentage.Value.FormatPercentage() : String.Empty;
        }

        public static string FormatPercentage(this decimal percentage)
        {
            return percentage.ToString("P2");
        }

        public static string FormatPercentage(this double? percentage)
        {
            return percentage.HasValue ? percentage.Value.FormatPercentage() : String.Empty;
        }

        public static string FormatPercentage(this double percentage)
        {
            return percentage.ToString("P2");
        }

        public static string FormatString(this object value)
        {
            return (value != null) ? value.ToString() : String.Empty;
        }

        public static DateTime ParseDate(this string date, string format = null)
        {
            return !format.IsNullOrBlank() ? DateTime.ParseExact(date, format, CultureInfo.InvariantCulture) : DateTime.Parse(date);
        }

        public static object Format(this object value, string format)
        {
            if (format.IsNullOrBlank()) throw new ArgumentNullException("format");
            if (value == null) return null;

            Type type = value.GetType();
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.String:
                    return value.ToString();
                case TypeCode.Byte:
                    Byte @byte = (Byte) value;
                    return Byte.Parse(@byte.ToString(format)); 
                case TypeCode.SByte:
                    SByte @sbyte = (SByte) value;
                    return SByte.Parse(@sbyte.ToString(format));
                case TypeCode.UInt16:
                    UInt16 @unit16 = (UInt16) value;
                    return UInt16.Parse(@unit16.ToString(format));
                case TypeCode.UInt32:
                    UInt32 @unit32 = (UInt32) value;
                    return UInt32.Parse(@unit32.ToString(format));
                case TypeCode.UInt64:
                    UInt64 @unit64 = (UInt64) value;
                    return UInt64.Parse(@unit64.ToString(format));
                case TypeCode.Int16:
                    Int16 @int16 = (Int16)value;
                    return Int16.Parse(@int16.ToString(format));
                case TypeCode.Int32:
                    Int32 @int32 = (Int32) value;
                    return Int32.Parse(@int32.ToString(format));
                case TypeCode.Int64:
                    Int64 @int64 = (Int64)value;
                    return Int16.Parse(@int64.ToString(format));                    
                case TypeCode.Decimal:
                    Decimal @decimal = (Decimal) value;
                    return Decimal.Parse(@decimal.ToString(format));
                case TypeCode.Double:
                    Double @double = (Double) value;
                    return Double.Parse(@double.ToString(format));
                case TypeCode.Single:
                    Single @single = (Single) value;
                    return Single.Parse(@single.ToString(format));                    
                case TypeCode.DateTime:
                    DateTime @datetime = (DateTime) value;
                    return DateTime.Parse(@datetime.ToString(format));
                case TypeCode.Object:
                    if (type.Equals(typeof(Guid)))
                    {
                        Guid @guid = (Guid) value;
                        return Guid.Parse(@guid.ToString(format));
                    }
                    else
                    {
                        throw new NotSupportedException(String.Format("Parsing operation Not Supportted for Type '{0}'.", type));
                    }
                default:
                    return value;
            }
        }
		
        #endregion
    }
}
