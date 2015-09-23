using DXTest.App_Data;
using DXTest.Code.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXTest.Models
{
    public class ViewModelGenerator
    {
        public static XmlEditorModel GetXmlEditorModel()
        {
            XmlEditorModel model = new XmlEditorModel();

            model.TreeModel = GetXmlTreeModel();
            model.DatabaseFiles = DatabaseManager.GetAllFiles();

            return model;
        }

        public static XmlTreeModel GetXmlTreeModel()
        {
            XmlDataProvider data = new XmlDataProvider();

            XmlTreeModel treeModel = new XmlTreeModel();
            treeModel.XmlNodes = data.GenerateXmlTreeNodes();
            treeModel.Namespaces = data.GetNamespaceManager().GetNamespacePrefixes();

            return treeModel;
        }
    }
}