using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Linq;

namespace DXTest.Code.Xml
{
    /// <summary>
    /// Is responsible for loading XML documents and report errors to the user
    /// </summary>
    static class XmlLoader
    {
        public static XDocument LoadXmlDocument(string path)
        {
            // Loads the XML text into a XmlDocument class
            XDocument document = null;
            try
            {
                using (var fileStream = new FileStream(path, FileMode.Open))
                {
                    document = XDocument.Load(fileStream);
                }
            }
            catch (XmlException)
            {
                // TODO: Show error
                return null;
            }
            return document;
        }
    }
}
