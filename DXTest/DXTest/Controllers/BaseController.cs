using DXTest.Code.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DXTest.Controllers
{
    /// <summary>
    /// A base controller that all other controlers inharit from. It just contains a property with the XmlDataProvider
    /// </summary>
    public class BaseController : Controller
    {
        public XmlDataProvider XmlDataProvider { get; set; }

        public BaseController()
        {
            XmlDataProvider = new XmlDataProvider();
        }
    }
}