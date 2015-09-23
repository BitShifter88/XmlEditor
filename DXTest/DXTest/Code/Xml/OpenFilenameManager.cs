using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXTest.Code.Xml
{
    /// <summary>
    /// The UI needs to keep track of the name of the opened file. This class is responsible for storing that information
    /// </summary>
    public class OpenFilenameManager
    {
        const string FILENAME = "Filename";

        public static void SetOpenFilename(string filename)
        {
            HttpContext.Current.Session[FILENAME] = filename;
        }
    
        public static string GetOpenFilename()
        {
            if (HttpContext.Current.Session[FILENAME] == null)
                return string.Empty;
            else
                return (string)HttpContext.Current.Session[FILENAME];
        }
    }
}