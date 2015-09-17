using DXTest.App_Data;
using DXTest.Code.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXTest.Models
{
    public class XmlEditorModel
    {
        public XmlTreeModel TreeModel { get; set; }
        public List<XmlFileEntry> DatabaseFiles { get; set; }
    }
}