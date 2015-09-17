using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXTest.Code.Xml
{
    /// <summary>
    /// When the user opens an XML file from hes local computer, this class checks wheater the file is an XML file
    /// </summary>
    public class XmlFileValidation
    {
        public static UploadControlValidationSettings Settings = new UploadControlValidationSettings()
        {
            AllowedFileExtensions = new string[] { ".xml" },
            MaxFileSize = 4194304
        };
    }
}