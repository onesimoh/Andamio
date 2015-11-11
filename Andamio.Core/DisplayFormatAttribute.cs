using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Andamio
{
    public class DisplayFormatAttribute : Attribute
    {
        #region Constructors
        public DisplayFormatAttribute()
            : base()
        {

        }

        public DisplayFormatAttribute(string displayName)
            : base()
        {
            if (displayName.IsNullOrBlank())
            { throw new ArgumentNullException("displayName"); }
            DisplayName = displayName;
        }

        #endregion

        #region Properties
        public string DisplayName { get; set; }
        public string DataFormatString { get; set; }
        public string NullDisplayText { get; set; }
        public int SortOrder { get; set; }
 
        #endregion

        #region NameValueCollection
        public static NameValueCollection ToNameValueCollection(object instance)
        {
            if (instance == null)
            { throw new ArgumentNullException("instance"); }

            NameValueCollection values = new NameValueCollection();

            var properties = instance.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            if (properties != null && properties.Any())
            {
                foreach (PropertyInfo property in properties)
                {
                    DisplayFormatAttribute displayFormatAttribute = property.GetCustomAttribute<DisplayFormatAttribute>();
                    if (displayFormatAttribute == null)
                    { continue; }

                    string displayName = !displayFormatAttribute.DisplayName.IsNullOrBlank() ? displayFormatAttribute.DisplayName : property.Name;
                    
                    string displayValue;
                    object propertyValue = property.GetValue(instance, null);
                    if ((propertyValue == null) && (displayFormatAttribute.NullDisplayText != null))
                    {
                        displayValue = displayFormatAttribute.NullDisplayText;
                    }
                    else if ((propertyValue != null) && (!displayFormatAttribute.DataFormatString.IsNullOrBlank()))
                    {
                        displayValue = String.Format(displayFormatAttribute.DataFormatString, propertyValue);
                    }
                    else if (propertyValue != null)
                    {
                        displayValue = propertyValue.ToString();
                    }
                    else
                    {
                        displayValue = null;
                    }

                    values.Add(displayName, displayValue);
                }
            }

            return values;
        }

        #endregion
    }
}
