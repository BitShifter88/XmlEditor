using DevExpress.Web.ASPxTreeList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXTest.Code.Xml
{
    public class XmlTreeNodeValidator
    {
        const string VALIDATION_ERRORS = "ValidationErrors";

        public static List<string> ValidateTreeNode(XmlTreeNode node)
        {
            List<string> errors = new List<string>();

            if (XmlHelper.IsStringLegalXmlName(node.Name) == false)
                errors.Add("Invalid XML name. XML names are not allowed to contain \":\" or whitespace. ");

            if (node.Type == XmlNodeType.Namespace && XmlHelper.IsStringLegalNamespaceUri(node.Name) == false)
            {
                errors.Add("Namespace declarations must have a value.");
            }

            HttpContext.Current.Session[VALIDATION_ERRORS] = errors;

            return errors;
        }

        public static void Validate(object sender,TreeListNodeValidationEventArgs e)
        {
            List<string> errors = (List<string>)HttpContext.Current.Session[VALIDATION_ERRORS];

            if (errors == null)
                return;

            e.NodeError = "";
            foreach (string error in errors)
            {
                e.NodeError += error;
            }
        }
    }
}