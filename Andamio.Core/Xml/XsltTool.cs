using System;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Xml.Serialization;
using System.Xml.Linq;

namespace Andamio.Xml
{
    /// <summary>
    /// Xslt Utility class.
    /// </summary>
    public static class XsltExtensions
    {
        /// <summary>
        /// Transform an xslt template using a specified object.
        /// </summary>
        /// <param name="xsltTemplate">The path to the xslt template.</param>
        /// <param name="args">Contains the namespace-qualified arguments used as input to the transform.</param>
        /// <param name="entity">The entity to be applied to the xslt template.</param>
        /// <returns>Returns the transformed xslt after the specified object is applied.</returns>
        public static string XsltTransform(this string xsltTemplate, XsltArgumentList args, object entity)
        {
            using (XmlReader xsltTemplateReader = XmlReader.Create(xsltTemplate, new XmlReaderSettings()))
            {
                return xsltTemplateReader.XsltTransform(args, entity);
            }
        }

        /// <summary>
        /// Transform an xslt template using a specified object.
        /// </summary>
        /// <param name="xsltTemplate">Xslt template.</param>
        /// <param name="args">Contains the namespace-qualified arguments used as input to the transform.</param>
        /// <param name="entity">The entity to be applied to the xslt template.</param>
        /// <returns>Returns the transformed xslt after the specified object is applied.</returns>
        public static string XsltTransform(this XmlReader xsltTemplate, XsltArgumentList args, object entity)
        {
            // Serialize entity to Xml String to apply Xslt.
            using (Stream stream = new MemoryStream())
            {
                XmlSerializer serializer = new XmlSerializer(entity.GetType());
                serializer.Serialize(stream, entity);
                stream.Seek(0, SeekOrigin.Begin);

                return xsltTemplate.XsltTransform(XDocument.Load(stream).Root);
            }
        }

        /// <summary>
        /// Transform an xslt template using a specified object.
        /// </summary>
        /// <param name="xsltTemplate">Xslt template.</param>
        /// <param name="args">Contains the namespace-qualified arguments used as input to the transform.</param>
        /// <param name="entity">The entity to be applied to the xslt template.</param>
        /// <returns>Returns the transformed xslt after the specified object is applied.</returns>
        public static string XsltTransform(this XmlReader xsltTemplate, XsltArgumentList args, string serializedEntity)
        {
            using (StringReader stringReader = new StringReader(serializedEntity))
            {
                using (XmlReader xmlReader = XmlReader.Create(stringReader))
                {
                    return xsltTemplate.XsltTransform(XDocument.Load(xmlReader).Root);
                }
            }            
        }

        /// <summary>
        /// Transform an xslt template using a specified object.
        /// </summary>
        /// <param name="xsltTemplate">Xslt template.</param>
        /// <param name="args">Contains the namespace-qualified arguments used as input to the transform.</param>
        /// <param name="entity">The entity to be applied to the xslt template.</param>
        /// <returns>Returns the transformed xslt after the specified object is applied.</returns>
        public static string XsltTransform(this XmlReader xsltTemplate, XsltArgumentList args, XElement xmlElement)
        {
            // Create the output stream
            StringBuilder output = new StringBuilder();
            using (XmlWriter xmlWriter = XmlWriter.Create(output))
            {
                //Load Xslt
                XslCompiledTransform xslTrans = new XslCompiledTransform();
                xslTrans.Load(xsltTemplate);

                // Transform Xml
                using (XmlReader xmlReader = xmlElement.CreateReader())
                {
                    xslTrans.Transform(xmlReader, args, xmlWriter);
                }
            }

            return output.ToString();
        }

        /// <summary>
        /// Transform an xslt template using a specified object.
        /// </summary>
        /// <param name="xsltTemplate">Xslt template.</param>
        /// <param name="entity">The entity to be applied to the xslt template.</param>
        /// <returns>Returns the transformed xslt after the specified object is applied.</returns>
        public static string XsltTransform(this XmlReader xsltTemplate, XElement xmlElement)
        {
            XsltArgumentList args = new XsltArgumentList();
            args.AddExtensionObject("urn:efg-formatting-extensions", new XsltExtension());
            return xsltTemplate.XsltTransform(args, xmlElement);
        }

        /// <summary>
        /// Transform an xslt template using a specified object.
        /// </summary>
        /// <param name="xsltTemplate">The path to the xslt template.</param>
        /// <param name="entity">The entity to be applied to the xslt template.</param>
        /// <returns>Returns the transformed xslt after the specified object is applied.</returns>
        public static string XsltTransform(this string xsltTemplate, object entity)
        {
            XsltArgumentList args = new XsltArgumentList();
            args.AddExtensionObject("urn:efg-formatting-extensions", new XsltExtension());
            return XsltTransform(xsltTemplate, args, entity);
        }

        /// <summary>
        /// Transform an xslt template using a specified object.
        /// </summary>
        /// <param name="xsltTemplate">Xslt template.</param>
        /// <param name="entity">The entity to be applied to the xslt template.</param>
        /// <returns>Returns the transformed xslt after the specified object is applied.</returns>
        public static string XsltTransform(this XmlReader xsltTemplate, object entity)
        {
            XsltArgumentList args = new XsltArgumentList();
            args.AddExtensionObject("urn:efg-formatting-extensions", new XsltExtension());
            return XsltTransform(xsltTemplate, args, entity);
        }        
    }
}
