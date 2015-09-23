using DevExpress.Web;
using DevExpress.Web.Mvc;
using DXTest.Code;
using DXTest.Code.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Xml.Linq;

namespace DXTest.Controllers
{
    /// <summary>
    /// This controller is used to handle responses from the DevExpress FileUpload controller. 
    /// </summary>
    public class FileController : BaseController
    {
        public ActionResult UploadControlUpload()
        {
            string[] errors;

            // Gets the XML file that the user chose to upload. The file is validated using the XmlFileValidation, and then FileUploadComplete is called.
            UploadControlExtension.GetUploadedFiles(
                "UploadControl",
                XmlFileValidation.Settings, out errors,
                FileUploadComplete,
                FilesUploadComplete);

            return null;
        }

        public void FileUploadComplete(object sender, FileUploadCompleteEventArgs e)
        {
            UploadedFile file = e.UploadedFile;

            // If the file turned out to be valid
            if (file.IsValid)
            {
                OpenFilenameManager.SetOpenFilename("");
                XDocument doc = null;
                try
                {
                    // We construct an XDocument from the XML text in the file
                    doc = XDocument.Load(file.FileContent);
                }
                catch
                {
                    return;
                }
                // We inform the data provider about the new XML data
                XmlDataProvider.SaveXDocument(doc);
                OpenFilenameManager.SetOpenFilename(file.FileName);
            }
        }

        public void FilesUploadComplete(object sender, FilesUploadCompleteEventArgs e)
        {

        }

        /// <summary>
        /// When the user wants to download the open XML document down to hes local computer as a file, this method is used to create the MVC FileResult
        /// </summary>
        /// <returns></returns>
        public FileResult DownloadXmlFile()
        {
            byte[] fileBytes = XmlHelper.SerializeXmlText(XmlHelper.XDocumenToString(XmlDataProvider.GetXDocument()));

            string fileName = "xml.xml";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
    }
}