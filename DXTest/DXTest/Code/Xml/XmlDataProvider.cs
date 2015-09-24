using System;
using System.Collections.Generic;
using System.Linq;
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
        IXmlDataSource _dataSource;

        public XmlDataProvider()
        {
            _dataSource = new SessionDataSource();
        }

        /// <summary>
        /// Used to set the XML data when the user loads an XML file from ether a file or database
        /// </summary>
        /// <param name="doc"></param>
        public void SaveXDocument(XDocument doc)
        {
            // NamespaceManager keeps track of the different namespaces in the curret XML document.
            NamespaceManager nm = new NamespaceManager();
            // Because of this, the NamespaceManager needs to go through the XDocument to scan it for namespace declaration
            nm.CheckXDocumentForNamespaces(doc);

            _dataSource.SetXDocument(doc);
            _dataSource.SetNamespaceManager(nm);
        }

        /// <summary>
        /// Gets the list of XmlTreeNodes used by the DevExpress TreeList to visually represent the XML data
        /// </summary>
        /// <returns></returns>
        public List<XmlTreeNode> GenerateXmlTreeNodes()
        {
            // If no document has been loaded, just create a default document
            if (_dataSource.GetXDocument() == null)
            {
                NewDefaultDocument();
            }

            List<XmlTreeNode> treeNodes = XmlToTreeListModel.GetTreeList(_dataSource.GetXDocument());
            _dataSource.SetXmlTreeNodes(treeNodes);

            return treeNodes;
        }

        /// <summary>
        /// Creates a new XDocument that contains a single root node
        /// </summary>
        public void NewDefaultDocument()
        {
            XDocument doc = new XDocument();
            XElement root = new XElement("Root", "");
            doc.Add(root);

            SaveXDocument(doc);
        }

        public XDocument GetXDocument()
        {
            return _dataSource.GetXDocument();
        }

        public NamespaceManager GetNamespaceManager()
        {
            return _dataSource.GetNamespaceManager();
        }

        public XmlTreeNode GetXmlTreeNode(int id)
        {
            return _dataSource.GetXmlTreeNodes().Where(node => node.Id == id).FirstOrDefault();
        }

        /// <summary>
        /// Updates the data for a specific XMl node
        /// </summary>
        /// <param name="updatedNode"></param>
        public void UpdateXmlData(XmlTreeNode updatedNode)
        {
            // If the new value of the updatedNode is null, the user wrote an empty value. We set the value of the updatedNode to empty string, since XDocument does not allow
            if (updatedNode.Value == null)
                updatedNode.Value = "";
            // The updated node that is sent back from the view does not contain an XObject. So we have to retrieve the node from the session
            List<XmlTreeNode> treeNodes = _dataSource.GetXmlTreeNodes();
            NamespaceManager namespaceManager = _dataSource.GetNamespaceManager();
            XDocument doc = GetXDocument();
            XmlTreeNode oldNode = treeNodes.Where(n => n.Id == updatedNode.Id).FirstOrDefault();

            if (oldNode.Type == XmlNodeType.Element)
            {
                UpdateElement(updatedNode, namespaceManager, oldNode);
            }
            else if (oldNode.Type == XmlNodeType.Attribute)
            {
                UpdateAttribute(updatedNode, oldNode);
            }
            else if (oldNode.Type == XmlNodeType.Namespace)
            {
                UpdateNamespaceDeclaration(updatedNode, namespaceManager, doc, oldNode);
            }
        }

        private static void UpdateNamespaceDeclaration(XmlTreeNode updatedNode, NamespaceManager namespaceManager, XDocument doc, XmlTreeNode oldNode)
        {
            XAttribute attribute = (XAttribute)oldNode.XObject;
            namespaceManager.ChangeNamespaceDeclaration(doc, attribute, oldNode.Parrent, updatedNode.Value, updatedNode.Name);
        }

        private static void UpdateAttribute(XmlTreeNode updatedNode, XmlTreeNode oldNode)
        {
            // XAttribute does not allow us to change its name. Therefor we have to replace th old attribute with a new one
            XAttribute attribute = (XAttribute)oldNode.XObject;
            XAttribute newAttribute = XmlHelper.CreateAttribute(XmlHelper.ChangeLocalNameForAttribute(attribute, updatedNode.Name), updatedNode.Value);
            XmlHelper.ReplaceAttribute(attribute, newAttribute, oldNode.Parrent);
        }

        private static void UpdateElement(XmlTreeNode updatedNode, NamespaceManager namespaceManager, XmlTreeNode oldNode)
        {
            XElement element = (XElement)oldNode.XObject;

            XmlHelper.ChangeLocalNameForElement(element, updatedNode.Name);

            // If the updated node has been assigned a namespace tag
            if (updatedNode.Tag != null)
            {
                XmlNamespace newNamespace = namespaceManager.GetNamespaceByPrefix(updatedNode.Tag);
                XmlHelper.ChangeNamespace(element, newNamespace);
            }

            // Parents don't have values, because XDocument does not like that
            if (oldNode.IsParrent == false)
                element.Value = updatedNode.Value;
        }

        /// <summary>
        /// The user can move XML nodes by using drag and drop. This method is used to perform the move
        /// </summary>
        /// <param name="id">The id of the node that is being moved</param>
        /// <param name="newParentId">The id of the new parent node</param>
        public void MoveXmlTreeNode(int id, int newParentId)
        {
            List<XmlTreeNode> treeNodes = _dataSource.GetXmlTreeNodes();
            // The node that is being moved
            XmlTreeNode node = treeNodes.Where(n => n.Id == id).FirstOrDefault();
            XmlTreeNode newParent = treeNodes.Where(n => n.Id == newParentId).FirstOrDefault();

            // Only elements can be parents
            if (newParent.Type != XmlNodeType.Element)
                return;

            XElement newParentElement = (XElement)newParent.XObject;

            if (node.Type == XmlNodeType.Element)
            {
                XElement element = (XElement)node.XObject;
                // Remove the element from its current position in the XML tree
                element.Remove();
                // And add it to it's new parent
                newParentElement.Add(element);
            }
            else if (node.Type == XmlNodeType.Attribute || node.Type == XmlNodeType.Namespace)
            {
                XAttribute attribute = (XAttribute)node.XObject;
                // Remove the attribute from its current position in the XML tree
                attribute.Remove();
                // Add it to its new position in the XMl tree
                newParentElement.Add(attribute);
            }
        }

        /// <summary>
        /// Deletes a node with the id, id
        /// </summary>
        /// <param name="id"></param>
        public void DeleteXmlTreeNode(int id)
        {
            List<XmlTreeNode> treeNodes = _dataSource.GetXmlTreeNodes();
            NamespaceManager namespaceManager = GetNamespaceManager();
            // The node that we want to delete
            XmlTreeNode node = treeNodes.Where(n => n.Id == id).FirstOrDefault();

            // We can't call .Remove() on the nodes XObject, so we have to cast it to an Element or Attribute in order to delete the node
            if (node.Type == XmlNodeType.Element)
            {
                XElement element = (XElement)node.XObject;
                element.Remove();
            }
            else if (node.Type == XmlNodeType.Attribute)
            {
                XAttribute attribute = (XAttribute)node.XObject;
                attribute.Remove();
            }
            else if (node.Type == XmlNodeType.Namespace)
            {
                XAttribute attribute = (XAttribute)node.XObject;
                // Namespaces also have to be removed from the namespace manager that keeps track of existing namespaces
                namespaceManager.RemoveNamespace(attribute, node.Parrent);
                attribute.Remove();
            }
        }


        /// <summary>
        /// Adds a new node to the curret XML document. A node is added to XDocument, and another one is added to the XmlTreeNode list that is used by the DevExpress TreeList to visually represent the XML document
        /// </summary>
        /// <param name="parrentId"></param>
        /// <param name="type"></param>
        public void AddNode(int parrentId, XmlNodeType type)
        {
            List<XmlTreeNode> treeNodes = _dataSource.GetXmlTreeNodes();
    
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

        /// <summary>
        /// Pastes the content of XmlTreeClipboard to the element with pasteTargetId id
        /// </summary>
        /// <param name="pasteTargetId"></param>
        /// <returns></returns>
        public bool Paste(int pasteTargetId)
        {
            XmlTreeNode parentNode = GetXmlTreeNode(pasteTargetId);
            XObject node = XmlTreeClipboard.Paste();
            // If no node has been copyed, or the target for the paste is an attribute, we return
            if (node == null || parentNode == null || parentNode.Type == XmlNodeType.Attribute)
                return false;

            XElement pasteTarget = (XElement)parentNode.XObject;

            // If the node that is being copyed is an Element
            if (node.GetType().IsAssignableFrom(typeof(XElement)))
            {
                XElement copyedNode = (XElement)node;
                pasteTarget.Add(copyedNode);
                // we make a copy again, so that we can do future pastes
                XmlTreeClipboard.Copy(new XElement(copyedNode));
            }
            // If the node that is being copyed is an attribute
            else if (node.GetType().IsAssignableFrom(typeof(XAttribute)))
            {
                XAttribute attribute = (XAttribute)node;
                pasteTarget.Add(attribute);
                // we make a copy again, so that we can do future pastes
                XmlTreeClipboard.Copy(attribute);
            }

            return true;
        }
    }
}