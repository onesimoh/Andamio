using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Andamio
{
    /// <summary>
    /// Extension methods for System.Enum
    /// </summary>
    public static class EnumExtensions
    {
        #region Enum Attribute
        /// <summary>
        /// Retrieves the display metadata associated with the enum value.
        /// </summary>
        /// <param name="enumValue">The enum value to inspect.</param>
        /// <returns>The requested metadata.</returns>
        public static EnumDisplayAttribute [] GetDisplayAttributes(this Enum enumValue)
        {
            return EnumDisplayAttribute.GetDisplayAttributes(enumValue);
        }
        
        /// <summary>
        /// Retrieves the display name of an enum value from the enum's metadata.  If the value is bit-masked,
        /// then the name of each flag will be separated with a comma.
        /// </summary>
        /// <param name="enumValue">The enum value to inspect.</param>
        /// <returns>The display name.</returns>
        public static string DisplayName(this Enum enumValue)
        {
            return enumValue.GetDisplayAttributes().Select(x => x.DisplayName).Concatenate();
        }

        ///// <summary>
        ///// Retrieves the short name of an enum value from the enum's metadata.  If the value is bit-masked,
        ///// then the short name of each flag will be separated with a comma.
        ///// </summary>
        ///// <param name="enumValue">The enum value to inspect.</param>
        ///// <returns>The short name.</returns>
        //public static string GetShortName(this Enum enumValue)
        //{
        //    return enumValue.GetDisplayAttributes().Select(x => x.ShortName).Join(", ");
        //}

        /// <summary>
        /// Retrieves the description of an enum value from the enum's metadata.  If the value is bit-masked,
        /// then the description of each flag will be separated with a newline.
        /// </summary>
        /// <param name="enumValue">The enum value to inspect.</param>
        /// <returns>The description.</returns>
        public static string GetDescription(this Enum enumValue)
        {
            return enumValue.GetDisplayAttributes().Select(x => x.Description).Concatenate();
        }

        /// <summary>
        /// Retrieves the sort order of an enum value from enum's metadata.  If the value is bit-masked,
        /// then the minimum sort order among all the flags will be returned.
        /// </summary>
        /// <param name="enumValue">The enum value to inspect.</param>
        /// <returns>The sort order.</returns>
        public static int GetSortOrder(this Enum enumValue)
        {
            return enumValue.GetDisplayAttributes().Select(x => x.SortOrder).Min();
        }

        /// <summary>
        /// Retrieves Selectable Flag from enum's metadata. If the value is bit-masked,
        /// then True will be returned if at least one of the values is selectable, False otherwise.
        /// </summary>
        /// <param name="enumValue">The enum value to inspect.</param>
        /// <returns>True of Enum contains a selectable vaue.</returns>
        public static bool IsSelectable(this Enum enumValue)
        {
            return enumValue.GetDisplayAttributes().Any(x => x.IsSelectable);
        }

        #endregion

        #region Parse
        /// <summary>
        /// Type safe enum parser that attempts to match both the enum name and display name (from metadata).  
        /// This operation is case-sensitive.
        /// </summary>
        /// <typeparam name="E">An enum type.</typeparam>
        /// <param name="value">The value to parse.</param>
        /// <returns>The corresponding enum value.</returns>
        public static E ParseEnum<E>(this string value)
        {
            return EnumDisplayAttribute.ParseEnum<E>(value, false);
        }

        /// <summary>
        /// Type safe enum parser that attempts to match both the enum name and display name (from metadata).  
        /// </summary>
        /// <typeparam name="E">An enum type.</typeparam>
        /// <param name="value">The value to parse.</param>
        /// <param name="ignoreCase">If true, then ignore case when comparing to the string value.</param>
        /// <returns>The corresponding enum value.</returns>
        public static E ParseEnum<E>(this string value, bool ignoreCase)
        {
            return EnumDisplayAttribute.ParseEnum<E>(value, ignoreCase);
        }
        public static E ParseEnum<E>(this string value, E defaultValue)
        {
            return EnumDisplayAttribute.ParseEnum<E>(value, defaultValue);
        }

        public static bool TryParseEnum<E>(this string value, out E enumValue)
        {
            return EnumDisplayAttribute.TryParseEnum<E>(value, out enumValue);
        }

        public static bool TryParseEnum(this string value, Type enumType, out object enumValue)
        {
            return EnumDisplayAttribute.TryParseEnum(value, enumType, out enumValue);
        }

        #endregion

        #region Separate Values
        public static string[] GetValueNames(this Enum enumValue)
        {
            //1) convert the enum value to a comma delimted string
            //2) split the string at commas
            //3) trim whitespace from each resulting name and return the array

            string commaString = enumValue.ToString();
            string[] splitStrings = commaString.Split(',');

            List<string> valueNames = new List<string>();
            foreach (string name in splitStrings)
            {
                valueNames.Add(name.Trim());
            }
            return valueNames.ToArray();
        }

        static public Enum[] GetValues(this Enum enumValue)
        {
            List<Enum> enumValues = new List<Enum>();
            string[] valueNames = enumValue.GetValueNames();
            if (valueNames.Any())
            { valueNames.ForEach(valueName => enumValues.Add((Enum)Enum.Parse(enumValue.GetType(), valueName))); }
            return enumValues.ToArray();
        }

        static public Enum[] GetSelectableValues(this Enum enumValue)
        {
            List<Enum> enumValues = new List<Enum>(enumValue.GetValues());
            enumValues.RemoveAll<Enum>(match => !match.IsSelectable());
            return enumValues.ToArray();
        }

		static public EnumT[] GetValues<EnumT>(this Enum enumValue)
			where EnumT : struct
		{
            List<EnumT> enumValues = new List<EnumT>();
            string[] valueNames = enumValue.GetValueNames();
            foreach (string valueName in valueNames)
            { 
                enumValues.Add(valueName.ParseEnum<EnumT>()); 
            }

			return enumValues.ToArray();
		}

        static public EnumT[] BitwiseSeparate<EnumT>(this Enum enumValue, bool omitThisValue = false, bool omitZeroValue = true)
            where EnumT : struct
        {
            Enum[] enumValues = enumValue.BitwiseSeparate(omitThisValue, omitZeroValue);
            return enumValues.Cast<EnumT>().ToArray();
        }

        public static Enum[] BitwiseSeparate(this Enum enumValue, bool omitThisValue = false, bool omitZeroValue = true)
        {
            Type enumType = enumValue.GetType();
            List<Enum> enumValues = new List<Enum>(); 

            int enumIntValue = Convert.ToInt32(enumValue);
            if (enumIntValue == 0 && !omitZeroValue)
            {
                enumValues.Add((Enum) Enum.Parse(enumType, enumValue.ToString()));
            }
            else
            {                
                foreach (int intValue in Enum.GetValues(enumType))
                {
                    if (omitZeroValue && intValue == 0)
                    { continue; }

                    if (omitThisValue && intValue == enumIntValue)
                    { continue; }

                    if ((intValue & enumIntValue) == intValue)
                    {
                        enumValues.Add((Enum) Enum.Parse(enumType, intValue.ToString()));
                    }
                }                
            }

            return enumValues.ToArray();
        }

        public static E[] ConvertEnumValuesTo<E>(this Enum enumValue) where E : struct
        {
            List<E> convertedEnumValues = new List<E>();
            Enum[] enumValues = enumValue.BitwiseSeparate();
            if (enumValues != null)
            {
                foreach (string enumStringValue in enumValues.Select(e => e.ToString()))
                {
                    E convertedEnumValue;
                    if (Enum.TryParse<E>(enumStringValue, true, out convertedEnumValue))
                    {
                        convertedEnumValues.Add(convertedEnumValue);
                    }
                }
            }

            return convertedEnumValues.ToArray();
        }

        public static int[] ToIntValues<E>(this IEnumerable<E> enumValues) where E : struct
        {
            return enumValues.Select(enumValue => Convert.ToInt32(enumValue)).ToArray();
        }

        #endregion

        #region Format
        public static string Format(this IEnumerable<Enum> enumValues)
        {
            string formattedString = String.Empty;
            if (enumValues != null && enumValues.Any())
            {
                formattedString = enumValues.Select(enumValue => enumValue.ToString()).JoinStrings(",");
            }

            return formattedString;
        }

        public static string Format(this Enum enumValue)
        {
            return enumValue.GetValueNames().JoinStrings(",");
        }


        #endregion

        #region Bitmask
        public static bool HasAnyFlag(this Enum enumValue, Enum flag)
        {
            return (Convert.ToInt32(enumValue) & Convert.ToInt32(flag)) != 0;
        }

        #endregion
    }
}
