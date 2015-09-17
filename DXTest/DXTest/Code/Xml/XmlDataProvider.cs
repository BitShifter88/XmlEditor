﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Xml.Linq;

namespace DXTest.Code.Xml
{
    /// <summary>
    /// This class is responsible for handling the data of the currently open XML file. The XML data is stored in the form of an XDocument, 
    /// and in the form of a list of XmlTreeNodes. The XDocument instance is used convert the XML data into a text string when the user wants to save the XML data.
    /// The list of XmlTreeNodes is used by the DevExpress TreeList to visually represent the XML data in a tree. The XDocument and list of XmlTreeNodes is stored in the Session dictionary
    /// </summary>
    public class XmlDataProvider
    {
        private const string XML_DOC_KEY = "XmlDoc";
        private const string XML_NAMESPACE_MANAGER = "XmlNamespaceManager";
        private const string XML_TREE_NODES = "XmlTreeNodes";

        /// <summary>
        /// Used to set the XML data when the user loads an XML file from ether a file or database
        /// </summary>
        /// <param name="doc"></param>
        public static void SetXmlData(XDocument doc)
        {
            HttpContext.Current.Session[XML_DOC_KEY] = doc;
            // NamespaceManager keeps track of the different namespaces in the curret XML document.
            NamespaceManager nm = new NamespaceManager();
            // Because of this, the NamespaceManager needs to go through the XDocument to scan it for namespace declaration
            nm.CheckXDocumentForNamespaces(doc);
            HttpContext.Current.Session[XML_NAMESPACE_MANAGER] = nm;
        }

        /// <summary>
        /// Gets the list of XmlTreeNodes used by the DevExpress TreeList to visually represent the XML data
        /// </summary>
        /// <returns></returns>
        public static List<XmlTreeNode> GetXmlTreeNodes()
        {
            XDocument doc = GetXDocumentFromSession();
            HttpContext.Current.Session[XML_TREE_NODES] = XmlToTreeListModel.GetTreeList(doc);

            return GetXmlTreeNodesFromSession();
        }

        public static XDocument GetXDocument()
        {
            return GetXDocumentFromSession();
        }

        private static XDocument GetXDocumentFromSession()
        {
            return (XDocument)HttpContext.Current.Session[XML_DOC_KEY];
        }

        public static NamespaceManager GetNamespaceManager()
        {
            if (HttpContext.Current.Session[XML_NAMESPACE_MANAGER] == null)
                HttpContext.Current.Session[XML_NAMESPACE_MANAGER] = new NamespaceManager();

            return (NamespaceManager)HttpContext.Current.Session[XML_NAMESPACE_MANAGER];
        }

        private static List<XmlTreeNode> GetXmlTreeNodesFromSession()
        {
            List<XmlTreeNode> treeNodes = (List<XmlTreeNode>)HttpContext.Current.Session[XML_TREE_NODES];

            //// If no nodes exist, no XML file is currently open. However, the DevExpress still needs a list of XmlTreeNodes, so we create an empty list of XmlTreeNodes
            //if (treeNodes == null)
            //{
            //    treeNodes = new List<XmlTreeNode>();
            //    HttpContext.Current.Session[XML_TREE_NODES] = treeNodes;
            //}

            return treeNodes;
        }

        public static XmlTreeNode GetXmlTreeNode(int id)
        {
            return GetXmlTreeNodesFromSession().Where(node => node.Id == id).FirstOrDefault();
        }

        /// <summary>
        /// Updates the data for a specific XMl node
        /// </summary>
        /// <param name="updatedNode"></param>
        public static void UpdateXmlData(XmlTreeNode updatedNode)
        {
            //// Updates the TreeList to ensure that the Dev Express TreeList shows a correct representation of the current XMl document
            //UpdateTreeListData(updatedNode);
            // Updates the XDocument with the changes represented by updateNode
            UpdateXDocumentData(updatedNode);
        }

        private static void UpdateXDocumentData(XmlTreeNode updatedNode)
        {
            // The updated node that is sent back from the view does not contain an XObject. So we have to retrieve the node from the session
            List<XmlTreeNode> treeNodes = GetXmlTreeNodesFromSession();
            XmlTreeNode node = treeNodes.Where(n => n.Id == updatedNode.Id).FirstOrDefault();

            if (node.Type == XmlNodeType.Element)
            {
                XElement element = (XElement)node.XObject;
                XmlHelper.ChangeLocalNameForElement(element, updatedNode.Name);

                // Parents don't have values, because XDocument does not like that
                if (node.IsParrent == false)
                    element.Value = updatedNode.Value;
            }
            else if (node.Type == XmlNodeType.Attribute)
            {
                XAttribute attribute = (XAttribute)node.XObject;
                XAttribute newAttribute = XmlHelper.CreateAttribute(XmlHelper.ChangeLocalNameForAttribute(attribute, updatedNode.Name), updatedNode.Value);
                XmlHelper.ReplaceAttribute(attribute, newAttribute, node.Parrent);
            }
        }

        /// <summary>
        /// Adds a new node to the curret XML document. A node is added to XDocument, and another one is added to the XmlTreeNode list that is used by the DevExpress TreeList to visually represent the XML document
        /// </summary>
        /// <param name="parrentId"></param>
        /// <param name="type"></param>
        public static void AddNode(int parrentId, XmlNodeType type)
        {
            List<XmlTreeNode> treeNodes = GetXmlTreeNodesFromSession();
    
            int highestId = treeNodes.OrderByDescending(i => i.Id).FirstOrDefault().Id;
            XmlTreeNode parent = treeNodes.Where(i => i.Id == parrentId).FirstOrDefault();
            XElement parentElement = (XElement)parent.XObject;

            // The default name and value of the new node
            string name = "Name" + treeNodes.Count;
            string value = "Value";

            // Creates the XObject representing the new node in the XML document
            XObject newNodeXObject;
            if (type == XmlNodeType.Element)
            {
                newNodeXObject = new XElement(name, value);
                // When an element becomes a parent, it is no longer allowed to have a value. XDocument does not support parent elements with values
                if (parentElement.Elements().Count() == 0)
                    parentElement.SetValue("");
            }
            else
                newNodeXObject = new XAttribute(name, value);
            parentElement.Add(newNodeXObject);

            //// Creates the XmlTreeNode that is added to the list of XmlTreeNodes used by the DevExpress TreeList to visually represented the XML document
            //XmlTreeNode node = new XmlTreeNode() { Id = highestId + 1, ParentId = parrentId, IsParrent = false, Name = "Name" + treeNodes.Count, Value = "Value", Type = type, Parrent = (XElement)parent.XObject, XObject = newNodeXObject };
            //treeNodes.Add(node);
        }

        //private static void UpdateTreeListData(XmlTreeNode updatedNode)
        //{
        //    List<XmlTreeNode> treeNodes = GetXmlTreeNodesFromSession();
        //    // Findes the updated node
        //    XmlTreeNode node = treeNodes.Where(n => n.Id == updatedNode.Id).FirstOrDefault();
        //    // Updates the values of the node with the values from updateNode
        //    if (node != null)
        //    {
        //        node.Name = updatedNode.Name;
        //        node.Value = updatedNode.Value;
        //        updatedNode.XObject = node.XObject;
        //        updatedNode.Parrent = node.Parrent;
        //    }
        //}
    }
}