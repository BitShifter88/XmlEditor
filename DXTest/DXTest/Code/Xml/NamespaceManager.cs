using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DXTest.Code.Xml
{
    public class XmlNamespace
    {
        public string Uri { get; set; }
        public string Prefix { get; set; }
    }

    /// <summary>
    /// This class is responsible for keeping track of the namespaces in the XML document. It is also responsible for adding/deleting and changing the name and value
    /// of namespaces. Changing the value and name of a namespace declaration is not trivial, since XAttribute handles this very poorly
    /// </summary>
    public class NamespaceManager
    {
        public List<XmlNamespace> Namespaces { get; set; }
        
        public NamespaceManager()
        {
            Namespaces = new List<XmlNamespace>();

            ClearNamespaces();
        }

        private void ClearNamespaces()
        {
            Namespaces.Clear();
            AddNamespace("None", "");
        }

        public List<string> GetNamespacePrefixes()
        {
            List<string> namespaces = new List<string>();

            foreach (XmlNamespace ns in Namespaces)
            {
                namespaces.Add(ns.Prefix);
            }

            return namespaces;
        }

        /// <summary>
        /// When an XML document is loaded, this methods scanns the XDocument for any namespace declarations. When it finds a namespace declation
        /// it is added to the Namespace attribute, in order to keep track of all namespaces.
        /// </summary>
        /// <param name="doc"></param>
        public void CheckXDocumentForNamespaces(XDocument doc)
        {
            ClearNamespaces();
            foreach (XElement node in doc.Elements())
            {
                CheckNodeForNamespace(node);
            }
        }

        /// <summary>
        /// Used to recursevly go through all XElments in an XDocument to scann for namespace declarations
        /// </summary>
        /// <param name="element"></param>
        private void CheckNodeForNamespace(XElement element)
        {
            foreach (XElement n in element.Elements())
            {
                CheckNodeForNamespace(n);
            }

            foreach (XAttribute a in element.Attributes())
            {
                if (a.IsNamespaceDeclaration)
                    AddNamespace(a.Name.LocalName, a.Value);
            }
        }

        /// <summary>
        /// Changes the prefix or URI of a namespace declaration. This is non-trivial since XAttribute does not support this by default.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="oldDec"></param>
        /// <param name="parrent"></param>
        /// <param name="newUri"></param>
        /// <param name="newPrefix"></param>
        /// <returns></returns>
        public XAttribute ChangeNamespaceDeclaration(XDocument doc, XAttribute oldDec, XElement parrent, string newUri, string newPrefix)
        {
            ChangeNamespace(oldDec.Name.LocalName, oldDec.Value, newPrefix, newUri);

            string oldNamespace = oldDec.Value;
            oldDec.Remove();

            newPrefix = newPrefix.Replace("xmlns:", "");
            XAttribute newAttribute = new XAttribute(XNamespace.Xmlns + newPrefix, newUri);

            parrent.Add(newAttribute);

            foreach (XElement element in doc.Elements())
            {
                ChangeNamespaceForElement(element, oldNamespace, newUri);
            }
            
            return newAttribute;
        }

        /// <summary>
        /// Changes the namespace an element is associated with.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="oldUri"></param>
        /// <param name="newUri"></param>
        private void ChangeNamespaceForElement(XElement element, string oldUri, string newUri)
        {
            if (element.Name.NamespaceName == oldUri)
            {
                if (newUri != string.Empty)
                    element.Name = "{" + newUri + "}" + element.Name.LocalName;
                else
                    element.Name = element.Name.LocalName;
            }

            foreach (XElement e in element.Elements())
            {
                ChangeNamespaceForElement(e, oldUri, newUri);
            }
        }

        /// <summary>
        /// Changes the prefix and/or URI of a namespace
        /// </summary>
        /// <param name="oldPrefix"></param>
        /// <param name="oldUri"></param>
        /// <param name="newPrefix"></param>
        /// <param name="newUri"></param>
        private void ChangeNamespace(string oldPrefix, string oldUri, string newPrefix, string newUri)
        {
            newPrefix = newPrefix.Replace("xmlns:", "");
            XmlNamespace ns = Namespaces.Where(i => i.Prefix == oldPrefix && i.Uri == oldUri).FirstOrDefault();
            if (ns == null)
                throw new Exception("Tried to change a namespace that did not exist");

            ns.Prefix = newPrefix;
            ns.Uri = newUri;
        }

        /// <summary>
        /// Takes an existing attribute and makes it a namespace declaration attribute
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public XAttribute MakeAttributeNamespaceDeclaration(XAttribute attribute, XElement parent)
        {
            XAttribute newAttribute = new XAttribute(XNamespace.Xmlns + attribute.Name.LocalName, attribute.Value);

            AddNamespace(newAttribute.Name.LocalName, attribute.Value);

            XmlHelper.ReplaceAttribute(attribute, newAttribute, parent);

            return newAttribute;
        }

        /// <summary>
        /// Take an attribute that fukctions as a namespace declaration, and makes it not be a namespace declaration
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public XAttribute MakeAttributeNotBeNamespaceDeclaration(XAttribute attribute, XElement parent)
        {
            RemoveNamespace(attribute, parent);
            attribute.Remove();

            XAttribute newAttribute = new XAttribute(attribute.Name.LocalName, attribute.Value);

            XmlHelper.AddAttributeToElement(newAttribute, parent);

            return newAttribute;
        }

        public void RemoveNamespace(XAttribute nsDeclaration, XElement parent)
        {
            ChangeNamespaceForElement(parent, nsDeclaration.Value, "");

            XmlNamespace ns = Namespaces.Where(i => i.Prefix == nsDeclaration.Name.LocalName && i.Uri == nsDeclaration.Value).FirstOrDefault();
            Namespaces.Remove(ns);
        }

        public void AddNamespace(string prefix, string uri)
        {
            XmlNamespace ns = new XmlNamespace() { Prefix = prefix, Uri = uri};
            Namespaces.Add(ns);
        }
    }
}
