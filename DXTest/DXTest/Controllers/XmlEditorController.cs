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
    public class XmlEditorController : Controller
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
            List<string> errors = XmlTreeNodeValidator.ValidateTreeNode(updatedNode);
            foreach (string error in errors)
            {
                ModelState.AddModelError(error, "");
            }
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

        [HttpPost]
        public ActionResult Paste(int id)
        {
            if (XmlDataProvider.Paste(id))
                return Json(false);
            else
                return Json(true);
        }

        [HttpPost]
        public ActionResult NodeDragDrop(int Id, int? ParentId)
        {
            if (ParentId == null)
                throw new Exception("Does this ever happen?");
            XmlDataProvider.MoveXmlTreeNode(Id, (int)ParentId);
            return PartialView("XmlTreePartial", ViewModelGenerator.GetXmlTreeModel());
        }

        [HttpPost]
        public ActionResult NewDocument()
        {
            XmlDataProvider.NewDocument();
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

        [HttpPost]
        public ActionResult OpenXmlFromDatabase(string filename)
        {
            XmlFile file = DatabaseManager.GetXmlfile(filename);
            string xmlText = XmlHelper.DeSerializeXmlText(file.Blob);
            XDocument doc = XmlHelper.StringToXDocument(xmlText);

            XmlDataProvider.SetXmlData(doc);

            OpenFilenameManager.SetOpenFilename(filename);

            return Json(true);
        }


        [HttpPost]
        public ActionResult UpdateOpenFilename()
        {
            return Json(OpenFilenameManager.GetOpenFilename());
        }

        [HttpPost]
        public ActionResult MakeNamespaceDeclaration(int focusedNodeKey)
        {
            XmlTreeNode node = XmlDataProvider.GetXmlTreeNode(focusedNodeKey);

            if (node.Type == XmlNodeType.Attribute)
            {
                XmlDataProvider.GetNamespaceManager().MakeAttributeNamespaceDeclaration((XAttribute)node.XObject, node.Parrent);
            }
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
            if (overwrite == false && DatabaseManager.DoesDatabaseContainXmlFile(filename) == false)
            {
                DatabaseManager.SaveNewXmlFileToDatabase(filename, XmlHelper.SerializeXmlText(XmlHelper.XDocumenToString(XmlDataProvider.GetXDocument())));
                return Json(true);
            }
            if (overwrite == true)
            {
                DatabaseManager.UpdateXmlFile(filename, XmlHelper.SerializeXmlText(XmlHelper.XDocumenToString(XmlDataProvider.GetXDocument())));
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }

    }
}