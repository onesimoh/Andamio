using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;

namespace Andamio
{
    /// <summary>
    /// Metadata for displaying enum values.
    /// </summary>
    [DebuggerDisplay("{DisplayName}")]
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class EnumDisplayAttribute : Attribute, IComparable<EnumDisplayAttribute>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the EnumDisplayAttribute class.
        /// </summary>
        /// <param name="displayName">The human readable name for the enum value.</param>
        public EnumDisplayAttribute(string displayName)
        {
            DisplayName = displayName;
            IsSelectable = true;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The human readable name for the enum value.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// A description of the enum value.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Indicator as to how values from the same enum should be sorted when displayed as a list.  Enum values
        /// are sorted first by SortOrder (small to large -- default of zero), then by display name (alphabetic).
        /// </summary>
        public int SortOrder  { get; set; }

        /// <summary>
        /// The name of the resource that can be used as an icon.
        /// </summary>
        public string IconResourceName  { get; set; }

        /// <summary>
        /// The name of the assembly that contains the embedded icon resource.  By default, the assembly that defines
        /// the enum is used.
        /// </summary>
        public string IconResourceAssembly { get; set; }

        /// <summary>
        /// Determines if specified Enum Value is selectable by users.
        /// </summary>
        public bool IsSelectable { get; set; }

        #endregion

        #region Methods
        /// <summary>
        /// Type safe enum parser that attempts to match both the enum name and display name (from metadata).  
        /// This operation is case-sensitive.
        /// </summary>
        /// <typeparam name="E">An enum type.</typeparam>
        /// <param name="value">The value to parse.</param>
        /// <returns>The corresponding enum value.</returns>
        public static E ParseEnum<E>(string value)
        {
            return ParseEnum<E>(value, false);
        }

        /// <summary>
        /// Type safe enum parser that attempts to match both the enum name and display name (from metadata).  
        /// </summary>
        /// <typeparam name="E">An enum type.</typeparam>
        /// <param name="value">The value to parse.</param>
        /// <param name="ignoreCase">If true, then ignore case when comparing to the string value.</param>
        /// <returns>The corresponding enum value.</returns>
        public static E ParseEnum<E>(string value, bool ignoreCase)
        {
            if (!typeof(E).IsEnum)
            { throw new ArgumentException("Generic type parameter must be an enum.", "E"); }
            if (String.IsNullOrEmpty(value))
            { throw new ArgumentException("value", "String must not be null or empty."); }

            // Attempt standard enumeration parsing
            try 
            { 
                return (E)Enum.Parse(typeof(E), value, ignoreCase); 
            }
            catch (ArgumentException) 
            { }

            //decide which type of comparison to use based on ignoreCase parameter
            StringComparison comparisonType = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

            string[] enumNames = Enum.GetNames(typeof(E));
            foreach (string enumName in enumNames)
            {
                //first, attempt to parse using the enum name
                if (value.Equals(enumName, comparisonType))
                { 
                    return (E)Enum.Parse(typeof(E), value, ignoreCase); 
                }
                else
                {
                    //second, attempt to parse using the enum display name
                    E enumValue = (E)Enum.Parse(typeof(E), enumName);
                    EnumDisplayAttribute displayInfo = EnumDisplayAttribute.GetDisplayAttributes(enumValue)[0];
                    if (value.Equals(displayInfo.DisplayName, comparisonType))
                    { 
                        return enumValue; 
                    }
                }
            }

            //if we get this far, then the value cannot be parsed
            string msg = string.Format("Could not parse the value '{0}' in Enum '{1}'"
                , value
                , typeof(E).ToString());
            throw new ArgumentException(msg, "value");
        }

        public static E ParseEnum<E>(string value, E defaultValue)
        {
            try
            {
                return ParseEnum<E>(value, ignoreCase: true);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static bool TryParseEnum(string value, Type enumType, out object enumValue)
        {
            var enumNames = Enum.GetNames(enumType);
            if (enumNames.Contains(value, StringComparer.OrdinalIgnoreCase))
            {
                enumValue = Enum.Parse(enumType, value, true);
                return true;
            }
            
            enumValue = null;
            return false;
        }

        public static bool TryParseEnum<E>(string value, out E enumValue)
        {
            try
            {
                enumValue = ParseEnum<E>(value, true);
                return true;
            }
            catch
            {
                enumValue = default(E);
                return false;
            }
        }

        /// <summary>
        /// Retrieves the display metadata for the given enum value.  If the enum was not decorated with the 
        /// EnumDisplay attribute, then an attribute containing default values is returned.  If the enum value
        /// is actually a bit-mask, then a collection of attributes will be returned which represents the individual
        /// flags set.
        /// </summary>        
        /// <param name="enumValue">The enum value to inspect.</param>
        /// <returns>The requested metadata.</returns>
        public static EnumDisplayAttribute[] GetDisplayAttributes(object enumValue)
        {
            List<EnumDisplayAttribute> displayAttributes = new List<EnumDisplayAttribute>();

            Type enumType = enumValue.GetType();
            if (!enumType.IsEnum)
            { throw new ArgumentException("Value must be an enum.", "enumValue"); }

            string[] enumValueNames = enumValue.ToString().Split(',');
            for (int i = 0; i < enumValueNames.Length; ++i)
            {
                string enumValueName = enumValueNames[i].Trim();

                //use reflection to get the FieldInfo for the given enum value                
                FieldInfo enumValueField = enumType.GetField(enumValueName);

                //get the EnumDisplayAttribute on the field (if any)                        
                object[] attributes = enumValueField.GetCustomAttributes(typeof(EnumDisplayAttribute), false);

                //if the attribute was found, then return it.  Otherwise, return a default attribute
                if (attributes.Length > 0)
                { 
                    displayAttributes.Add((EnumDisplayAttribute)attributes[0]); 
                }
                else
                { 
                    displayAttributes.Add(new EnumDisplayAttribute(enumValueName)); 
                }
            }

            return displayAttributes.ToArray();
        }

        /// <summary>
        /// Retrives a list of enum values in sorted order.
        /// </summary>
        /// <typeparam name="E">An enum type.</typeparam>
        /// <returns>A sorted list of enum values.</returns>
        public static IList<E> GetSortedEnums<E>(bool selectableOnly = false)
        {
            if (!typeof(E).IsEnum)
            { throw new ArgumentException("Generic type parameter must be an enum.", "E"); }

            SortedList<EnumDisplayAttribute, E> enumValues = new SortedList<EnumDisplayAttribute, E>();
            foreach (string enumName in Enum.GetNames(typeof(E)))
            {
                E enumValue = ParseEnum<E>(enumName);
                EnumDisplayAttribute displayInfo = GetDisplayAttributes(enumValue)[0];
                if (selectableOnly && !displayInfo.IsSelectable)
                { continue; }
                enumValues.Add(displayInfo, enumValue);                
            }
            return enumValues.Values;
        }

        /// <summary>
        /// Sorts a collection of enum values using the enum's metadata (sort order then display name).
        /// </summary>
        /// <typeparam name="E">An enum type</typeparam>
        /// <param name="values">A collection of enum values to sort.</param>
        /// <returns>A sorted list of enum values.</returns>
        public static IList<E> SortEnums<E>(IEnumerable<E> values)
        {
            if (!typeof(E).IsEnum)
            { throw new ArgumentException("Generic type parameter must be an enum.", "E"); }

            SortedList<EnumDisplayAttribute, E> sortedValues = new SortedList<EnumDisplayAttribute, E>();
            foreach (E value in values)
            {
                EnumDisplayAttribute displayInfo = GetDisplayAttributes(value)[0];
                sortedValues.Add(displayInfo, value);
            }

            return sortedValues.Values;
        }

        /// <summary>
        /// Retrives a list of enum display names in random order.
        /// </summary>
        /// <typeparam name="E">An enum type.</typeparam>
        /// <returns>A list of display names.</returns>
        public static string[] GetDisplayNames<E>()
        {
            if (!typeof(E).IsEnum)
            { throw new ArgumentException("Generic type parameter must be an enum.", "E"); }

            List<string> displayNames = new List<string>();
            foreach (string enumName in Enum.GetNames(typeof(E)))
            {
                E enumValue = ParseEnum<E>(enumName);
                EnumDisplayAttribute displayInfo = GetDisplayAttributes(enumValue)[0];
                displayNames.Add(displayInfo.DisplayName);
            }
            return displayNames.ToArray();
        }
        #endregion

        #region IComparable<EnumDisplayAttribute> Members
        /// <summary>
        /// Compares this EnumDisplayAttribute with another and returns an indication of their relative values.  Items
        /// are first compared by SortOrder and then by DisplayName.
        /// </summary>
        /// <param name="other">The object to compare to.</param>
        /// <returns>A signed number indicating the relative values of this instance compare to the other parameter.</returns>
        public int CompareTo(EnumDisplayAttribute other)
        {
            //compare first by SortOrder (small to large), and second by DisplayName (alphabetic)
            if (this.SortOrder != other.SortOrder)
            { 
                return this.SortOrder.CompareTo(other.SortOrder); 
            }
            else
            { 
                return this.DisplayName.CompareTo(other.DisplayName); 
            }
        }
        #endregion
    }
}
