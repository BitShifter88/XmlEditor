using DevExpress.Web.ASPxTreeList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXTest.Code.Xml
{
    /// <summary>
    /// When the user edits an XML node, this class is used to validate wheater the edit is valid.
    /// For instance, if the edit the name of a node using an illigal character like ":", this class will discover that and add errors to the TreeList controler for the user to see
    /// </summary>
    public class XmlTreeNodeValidator
    {
        const string VALIDATION_ERRORS = "ValidationErrors";

        /// <summary>
        /// Finds out if there are any validation errors in the edited node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Adds the validation errors to the DevExpress TreeList controler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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