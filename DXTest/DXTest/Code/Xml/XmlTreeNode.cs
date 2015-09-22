using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace DXTest.Code.Xml
{
    public enum XmlNodeType : int
    {
        Element = 0,
        Attribute = 1,
        Namespace = 2,
    }

    /// <summary>
    /// Used to create representation of the XML data that is then used by the DevExpress TreeList to visually draw the XML data in a tree 
    /// </summary>
    public class XmlTreeNode
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public bool IsParrent { get; set; }
        public string Tag { get; set; }
        public XmlNodeType Type { get; set; }

        public XObject XObject { get; set; }
        public XElement Parrent { get; set; }
    }
}