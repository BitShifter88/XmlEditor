using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace DXTest.Code.Xml
{
    public class XmlTreeClipboard
    {
        private const string CLIPBOARD = "Clipboard";

        public static void Copy(int nodeId)
        {
            XmlTreeNode node = XmlDataProvider.GetXmlTreeNode(nodeId);

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