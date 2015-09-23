using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace DXTest.Code.Xml
{
    /// <summary>
    /// This class provides a few helping methods for manipulating and validating xml data related to XDocument. The class is static and stateless
    /// </summary>
    static class XmlHelper
    {
        /// <summary>
        /// XML names must not contain spaces or be empty. This method checks of a given XML name is valid
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsStringLegalXmlName(string name)
        {
            // We try to create an XElement with the name. If it fails, the name is invalid
            try
            {
                XElement element = new XElement(name, "");
            }
            catch (XmlException)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Namespace URIs are not allowed to be empty
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static bool IsStringLegalNamespaceUri(string uri)
        {
            // Namespaces URI's can't be empty
            if (uri == string.Empty)
                return false;
            return true;
        }

        /// <summary>
        /// Serializes the an XML text into a byte array
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public static byte[] SerializeXmlText(string xmlText)
        {
            byte[] serializedXml = System.Text.Encoding.UTF8.GetBytes(xmlText);

            return serializedXml;
        }

        /// <summary>
        /// Deserializes xml data in the form of a byte array
        /// </summary>
        /// <param name="serializedXml"></param>
        /// <returns></returns>
        public static string DeSerializeXmlText(byte[] serializedXml)
        {
            string xmlText = System.Text.Encoding.UTF8.GetString(serializedXml);
            return xmlText;
        }

        /// <summary>
        /// The ToString() method in the XDocument class does not return all the XML text that the XDocument contains. What's missing is the start declaration. This method
        /// returns the code contained in the XDocument plus the start declaration.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public static string XDocumenToString(XDocument document)
        {
            if (document == null)
                return string.Empty;

            if (document.Declaration == null)
                return document.ToString();
            else
                // document.ToString does not convert the entire document to a sting, because the XML declaration will be missing. To remedy this, we manualy add the document.Declaration to the string
                return document.Declaration + System.Environment.NewLine + document.ToString();
        }

        /// <summary>
        /// Creates an XDocument from a string of XML text
        /// </summary>
        /// <param name="xmlText"></param>
        /// <returns></returns>
        public static XDocument StringToXDocument(string xmlText)
        {
            if (xmlText == string.Empty)
                return new XDocument();

            XDocument doc = XDocument.Load(GenerateStreamFromString(xmlText));

            return doc;
        }

        private static Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static void AddAttributeToElement(XAttribute attribute, XElement element)
        {
            // You can't just add an attribute to an element. You have to retrive the list of current attributes on an element, add the new attribute to the list, and then call ReplaceAttributes on element
            List<XAttribute> attributes = element.Attributes().ToList();
            attributes.Add(attribute);
            element.ReplaceAttributes(attributes);
        }

        public static void RemoveAttribute(XAttribute attribute, XElement element)
        {
            List<XAttribute> attributes = element.Attributes().ToList();
            attributes.Remove(attribute);
            element.ReplaceAttributes(attributes);
        }

        public static void ChangeLocalNameForElement(XElement element, string newLocalName)
        {
            element.Name = element.Name.Namespace + newLocalName;
        }

        public static string ChangeLocalNameForAttribute(XAttribute attribute, string newLocalName)
        {
            if (attribute.Name.NamespaceName != string.Empty)
            {
                string name = "{" + attribute.Name.Namespace.NamespaceName.ToString() + "}" + newLocalName;
                return name;
            }
            else
                return newLocalName;
        }

        public static void ChangeNamespace(XElement element, XmlNamespace ns)
        {
            if (ns.Uri != "")
                element.Name = "{" + ns.Uri + "}" + element.Name.LocalName;
            else
                element.Name = element.Name.LocalName;
        }
       
        public static XAttribute CreateAttribute(string name, string value)
        {
             return new XAttribute(name, value);
            
        }

        /// <summary>
        /// Replaces oldAttribute with newAttribute
        /// </summary>
        /// <param name="oldAttribute"></param>
        /// <param name="newAttribute"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public static bool ReplaceAttribute(XAttribute oldAttribute, XAttribute newAttribute, XElement element)
        {
            // Gets the attributes that element has
            List<XAttribute> attList = element.Attributes().ToList();
            // Find the index of the old attribute
            int indexSelectedAttribute = attList.IndexOf(oldAttribute);
            // Replace it with the new attribute
            attList[indexSelectedAttribute] = newAttribute;
            try
            {
                // Update the list of attribute on element, with the new list
                element.ReplaceAttributes(attList);
            }
            catch (System.InvalidOperationException)
            {
                // If the new attribute is invalid (e.g. the attribute name i already in use), we role back the changes
                attList.Remove(newAttribute);
                attList.Add(oldAttribute);
                element.ReplaceAttributes(attList);
                return false;
            }

            return true;
        }
    }
}
