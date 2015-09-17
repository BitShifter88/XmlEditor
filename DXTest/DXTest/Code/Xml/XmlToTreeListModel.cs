using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace DXTest.Code.Xml
{
    public static class XmlToTreeListModel
    {
        /// <summary>
        /// Takes an XDocument and creates a list of XmlTreeNodes corresponding to the structure in the XDocument.
        /// The list of XmlTreeNodes is used by the dev express TreeList controller to visually construct the tree view
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public static List<XmlTreeNode> GetTreeList(XDocument document)
        {
            List<XmlTreeNode> treeNodes = new List<XmlTreeNode>();

            if (document == null)
                return treeNodes;

            int nodeId = 1;
            // Recursively goes through every node in the XDocument, and constructs a list of XmlTreeNodes used to visually represent the XML data in the Dev Express TreeList
            foreach (XElement element in document.Elements())
            {
                treeNodes.AddRange(ProcessXElement(element, null, 0, ref nodeId));
            }

            return treeNodes;
        }

        private static List<XmlTreeNode> ProcessXElement(XElement element, XElement parrent, int parrentId, ref int nodeId)
        {
            List<XmlTreeNode> treeNodes = new List<XmlTreeNode>();

            XmlTreeNode treeNode = CreateTreeNodeFromElement(element, parrent, parrentId, ref nodeId);

            treeNodes.Add(treeNode);

            foreach (XElement child in element.Elements())
            {
                treeNodes.AddRange(ProcessXElement(child, element, treeNode.Id, ref nodeId));
            }
            foreach (XAttribute attribute in element.Attributes())
            {
                treeNodes.Add(CreateTreeNodeFromAttribute(attribute, treeNode.Id, element, ref nodeId));
            }

            return treeNodes;
        }

        private static XmlTreeNode CreateTreeNodeFromElement(XElement element, XElement parrent, int parrentId, ref int nodeId)
        {
            // Determines the text value of the element. If the element is a parrent, it has no text value
            string value = "";
            if (element.Elements().Count() == 0)
                value = element.Value;

            bool isParent = false;
            if (element.Elements().Count() > 0)
                isParent = true;

            return CreateTreeNode(parrentId, element.Name.LocalName.ToString(), value, XmlNodeType.Element, parrent, element, element.GetPrefixOfNamespace(element.Name.Namespace), isParent, ref nodeId);
        }

        private static XmlTreeNode CreateTreeNodeFromAttribute(XAttribute attribute, int parrentId, XElement parrent, ref int nodeId)
        {
            if (attribute.IsNamespaceDeclaration == false)
                return CreateTreeNode(parrentId, attribute.Name.LocalName.ToString(), attribute.Value, XmlNodeType.Attribute, parrent, attribute, "", false, ref nodeId);
            else
                return CreateTreeNode(parrentId, attribute.Name.LocalName.ToString(), attribute.Value, XmlNodeType.Namespace, parrent, attribute, "",false, ref nodeId);
        }

        public static XmlTreeNode CreateTreeNode(int parrentId, string name, string value, XmlNodeType type, XElement parrent, XObject xObject, string tag, bool isParent, ref int nodeId)
        {
            XmlTreeNode treeNode = new XmlTreeNode() {
                Id = nodeId,
                ParentId = parrentId,
                Name = name,
                Value = value,
                Type = type,
                XObject = xObject,
                Parrent = parrent,
                IsParrent = isParent,
                Tag = tag
            };

            // Increments the id counter, so that we can assign the id to the next tree node that will be created
            nodeId++;

            return treeNode;
        }
    }
}