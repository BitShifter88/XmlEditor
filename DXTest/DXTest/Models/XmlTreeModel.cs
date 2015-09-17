using DXTest.Code.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXTest.Models
{
    public class XmlTreeModel
    {
        public List<XmlTreeNode> XmlNodes { get; set; }
        public List<string> Namespaces { get; set; }
    }
}