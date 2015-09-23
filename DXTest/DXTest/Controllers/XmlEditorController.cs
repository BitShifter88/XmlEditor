using DXTest.App_Data;
using DXTest.Code.Xml;
using DXTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace DXTest.Controllers
{
    /// <summary>
    /// The main controller the XML web application. It controls the view and the parial views of the XMl web application
    /// </summary>
    public class XmlEditorController : BaseController
    {
        public ActionResult Index()
        {
            return View(ViewModelGenerator.GetXmlEditorModel());
        }

        public ActionResult XmlTreePartial()
        {
            return PartialView("XmlTreePartial", ViewModelGenerator.GetXmlTreeModel());
        }

        /// <summary>
        /// Called by the DevExpress TreeList when the user updates the data of an XML tree node
        /// </summary>
        /// <param name="updatedNode"></param>
        /// <returns></returns>
        [HttpPost, ValidateInput(false)]
        public ActionResult UpdateNode(XmlTreeNode updatedNode)
        {
            // A small hack because of a bug in the DevExpress TreeView. We call ValidateTreeNode to determine if the updatedNode contains invalid data
            List<string> errors = XmlTreeNodeValidator.ValidateTreeNode(updatedNode);
            // These errors are then added to the ModelState
            foreach (string error in errors)
            {
                ModelState.AddModelError(error, "");
            }
            // If no errors exist, we perform the update
            if (ModelState.IsValid)
            {
                XmlDataProvider.UpdateXmlData(updatedNode);
            }
            return PartialView("XmlTreePartial", ViewModelGenerator.GetXmlTreeModel());
        }

        [HttpPost]
        public ActionResult DeleteNode(int Id)
        {
            XmlDataProvider.DeleteXmlTreeNode(Id);
            return PartialView("XmlTreePartial", ViewModelGenerator.GetXmlTreeModel());
        }

        [HttpPost]
        public ActionResult DeleteNodeJson(int id)
        {
            XmlDataProvider.DeleteXmlTreeNode(id);
            return Json(true);
        }

        [HttpPost]
        public ActionResult Copy(int id)
        {
            XmlTreeClipboard.Copy(id);
            return Json(true);
        }

        [HttpPost]
        public ActionResult Cut(int id)
        {
            XmlTreeClipboard.Copy(id);
            XmlDataProvider.DeleteXmlTreeNode(id);
            return Json(true);
        }

        /// <summary>
        /// Pastes the content of the clipboard into the target node. It returns Json(false) if the paste in not allowed.
        /// </summary>
        /// <param name="id">The id of the target node</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Paste(int id)
        {
            if (XmlDataProvider.Paste(id))
                return Json(false);
            else
                return Json(true);
        }

        /// <summary>
        /// Called by the TreeList when the user performs a drag and drop opperation.
        /// </summary>
        /// <param name="Id">The node that is being draged</param>
        /// <param name="ParentId">The destenation of the draged node</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult NodeDragDrop(int Id, int? ParentId)
        {
            if (ParentId == null)
                throw new Exception("Does this ever happen?");
            XmlDataProvider.MoveXmlTreeNode(Id, (int)ParentId);
            return PartialView("XmlTreePartial", ViewModelGenerator.GetXmlTreeModel());
        }

        /// <summary>
        /// Creates a new default document
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult NewDocument()
        {
            XmlDataProvider.NewDefaultDocument();
            return Json(true);
        }

        public ActionResult SaveFileListPartial(string selectedFeatures)
        {
            return PartialView("SaveFileListPartial", DatabaseManager.GetAllFiles());
        }
        
        public ActionResult OpenFileListPartial(string selectedFeatures)
        {
            return PartialView("OpenFileListPartial", DatabaseManager.GetAllFiles());
        }

        /// <summary>
        /// Called when the UI requests an XML file from the database to be opened
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult OpenXmlFromDatabase(string filename)
        {
            XmlFile file = DatabaseManager.GetXmlfile(filename);
            string xmlText = XmlHelper.DeSerializeXmlText(file.Blob);
            XDocument doc = XmlHelper.StringToXDocument(xmlText);

            XmlDataProvider.SaveXDocument(doc);

            OpenFilenameManager.SetOpenFilename(filename);

            return Json(true);
        }

        /// <summary>
        /// The client UI can request the name of the open XML file
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateOpenFilename()
        {
            return Json(OpenFilenameManager.GetOpenFilename());
        }

        [HttpPost]
        public ActionResult MakeNamespaceDeclaration(int focusedNodeKey)
        {
            // Gets the node from the XmlDataProvider that will be made into a namespace declaration
            XmlTreeNode node = XmlDataProvider.GetXmlTreeNode(focusedNodeKey);

            // If the node is a normal attribute, it is made into a namespace
            if (node.Type == XmlNodeType.Attribute)
            {
                XmlDataProvider.GetNamespaceManager().MakeAttributeNamespaceDeclaration((XAttribute)node.XObject, node.Parrent);
            }
            // But if the node is already a namespace declaretion, the node is made into a normal attribute
            else if (node.Type == XmlNodeType.Namespace)
            {
                XmlDataProvider.GetNamespaceManager().MakeAttributeNotBeNamespaceDeclaration((XAttribute)node.XObject, node.Parrent);
            }
            return Json(true);
        }

        [HttpPost]
        public ActionResult AddNode(int focusedNode, XmlNodeType type)
        {
            XmlDataProvider.AddNode(focusedNode, type);
            return Json(true);
        }

        [HttpPost]
        public ActionResult SaveXmlToDatabase(string filename, bool overwrite)
        {
            // If the database does not contain an XML file with the requested filename, all is well and we save the XML file to the database
            if (DatabaseManager.DoesDatabaseContainXmlFile(filename) == false)
            {
                DatabaseManager.SaveNewXmlFileToDatabase(filename, XmlHelper.SerializeXmlText(XmlHelper.XDocumenToString(XmlDataProvider.GetXDocument())));
                return Json(true);
            }
            // Else if we are allowed to overwrite any existing XML files with that filename, we overwrite it
            else if (overwrite == true)
            {
                DatabaseManager.UpdateXmlFile(filename, XmlHelper.SerializeXmlText(XmlHelper.XDocumenToString(XmlDataProvider.GetXDocument())));
                return Json(true);
            }
            // Else, an XML file with the that filename already exists, and we are not allowed to overwrite it.. So we return false, making the UI ask the user if he wants to overwrite
            else
            {
                return Json(false);
            }
        }

    }
}