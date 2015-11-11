using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;

namespace Andamio.Xml
{
    /// <summary>
    /// Xslt Extension Object containing various formatting functions.
    /// </summary>
    /// <remarks>Developers are encouraged to extend the functionality of this class by providing addtional methods.</remarks>
    /// <example>
    /// The example below demonstrates how to use XsltStringUtilsExtension along with XsltUtil Utility class.
    /// <code lang="cs">
    /// XsltStringUtilsExtension converter = new XsltStringUtilsExtension();
    /// XsltArgumentList args = new XsltArgumentList();
    /// args.AddExtensionObject( "urn:converter", converter );
    /// 
    /// XsltUtil.Transform( "template.xslt", args, entityValue );
    /// </code>
    /// </example>
    public class XsltExtension
    {
        /// <summary>
        /// Formats a string representation of a DateTime value.
        /// </summary>
        /// <param name="value">String representation of the DateTime value to format.</param>
        /// <param name="format">Format to be applied to DateTime value.</param>
        /// <returns>Returns a formatted string of the specified DateTime value.</returns>         
        public string FormatDate(DateTime value)
        {
            return value.ToString("d");
        }

        /// <summary>
        /// Formats a string representation of a DateTime value.
        /// </summary>
        /// <param name="value">String representation of the DateTime value to format.</param>
        /// <param name="format">Format to be applied to DateTime value.</param>
        /// <returns>Returns a formatted string of the specified DateTime value.</returns>         
        public string FormatStringDate(string dateValue)
        {
            DateTime dateTime;
            if (DateTime.TryParse(dateValue, out dateTime))
            {
                return dateTime.ToString("d");
            }
            return String.Empty;
        }

        /// <summary>
        /// Formats a string representation of a decimal value.
        /// </summary>
        /// <param name="value">String representation of the decimal value to format.</param>
        /// <param name="format">Format to be applied to decimal value.</param>
        /// <returns>Returns a formatted string of the specified decimal value.</returns>
        public string DecimalString(string value, string format)
        {
            decimal num = Decimal.Parse(value);
            return num.ToString(format);
        }

        /// <summary>
        /// Formats currency value to format $#,###.## (ex. $1,000.00)
        /// </summary>
        public string FormatCurrency(decimal amount)
        {
            return amount.ToString("C");
        }


        public string GetFileContent(string relativeWebPath)
        {
            using (FileStream stream = File.OpenRead(HttpContext.Current.Server.MapPath(relativeWebPath)))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
