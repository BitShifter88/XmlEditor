using System.Collections.Generic;
using System.Xml.Linq;

namespace DXTest.Code.Xml
{
    /// <summary>
    /// When a user opnes an XML file from the database, or his local computer, the XML data is loaded by the XmlDataProvider class.
    /// However, the XmlDataProvider needs somewhere to physically store this data while the file is opened.
    /// This project comes with a default data source in the form of the SessionDataSource class, that stores the XML file data in the ASP.NET Session variable.
    /// This interface can be used to implement other data sources if necessary. For instance, the ASP.NET Session variable can be a bit unreliable in some cases,
    /// so depending on the situation, it might be beneficial to implement a class that uses a more persistent data source, for instance a SQL Server database 
    /// </summary>
    public interface IXmlDataSource
    {
        NamespaceManager GetNamespaceManager();
        XDocument GetXDocument();
        List<XmlTreeNode> GetXmlTreeNodes();
        void SetNamespaceManager(NamespaceManager nm);
        void SetXDocument(XDocument doc);
        void SetXmlTreeNodes(List<XmlTreeNode> nodes);
    }
}