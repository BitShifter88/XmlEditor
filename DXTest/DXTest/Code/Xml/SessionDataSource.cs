using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace DXTest.Code.Xml
{
    /// <summary>
    /// When an XML document is opened by the user, it has to be store somewhere. This class is used to store that data in the ASP.NET Session variable
    /// If the Session variable is deemed to be to volatile, another data source can be created by creating a new class that implements the IXmlDataSource
    /// </summary>
    public class SessionDataSource : IXmlDataSource
    {
        private const string XML_DOC_KEY = "XmlDoc";
        private const string XML_NAMESPACE_MANAGER = "XmlNamespaceManager";
        private const string XML_TREE_NODES = "XmlTreeNodes";

        public void SetXDocument(XDocument doc)
        {
            HttpContext.Current.Session[XML_DOC_KEY] = doc;
        }

        public void SetNamespaceManager(NamespaceManager nm)
        {
            HttpContext.Current.Session[XML_NAMESPACE_MANAGER] = nm;
        }

        public void SetXmlTreeNodes(List<XmlTreeNode> nodes)
        {
            HttpContext.Current.Session[XML_TREE_NODES] = nodes;
        }

        public XDocument GetXDocument()
        {
            return (XDocument)HttpContext.Current.Session[XML_DOC_KEY];
        }

        public NamespaceManager GetNamespaceManager()
        {
            return (NamespaceManager)HttpContext.Current.Session[XML_NAMESPACE_MANAGER];
        }

        public List<XmlTreeNode> GetXmlTreeNodes()
        {
            return (List<XmlTreeNode>)HttpContext.Current.Session[XML_TREE_NODES];
        }
    }
}