using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace DXTest.Code.Xml
{
    /// <summary>
    /// The user can copy, cut and paste XML nodes in the TreeView. This class serves as a clipboard for that. It is not possible to store data in the OS clipboard on the client due to security reasons
    /// The ASP.NET Session variable is used to store the copyied data.
    /// </summary>
    public class XmlTreeClipboard
    {
        private const string CLIPBOARD = "Clipboard";

        public static void Copy(int nodeId)
        {
            XmlTreeNode node = new XmlDataProvider().GetXmlTreeNode(nodeId);

            if (node.Type == XmlNodeType.Element)
            {
                // Makes a deep copy of the nodes XElement and stores it in the Session
                Copy(new XElement((XElement)node.XObject));
            }
            else if (node.Type == XmlNodeType.Attribute)
            {
                // Makes a deep copy of the nodes XAttribute and stores it in the Session
                Copy(new XAttribute((XAttribute)node.XObject));
            }
            else
                throw new ArgumentException("Unkown node type");
        }
        public static void Copy(XObject obj)
        {
            HttpContext.Current.Session[CLIPBOARD] = obj;
        }

        public static XObject Paste()
        {
            if (HttpContext.Current.Session[CLIPBOARD] != null)
            {
                return (XObject)HttpContext.Current.Session[CLIPBOARD];
            }
            else
                return null;

        }
    }
}