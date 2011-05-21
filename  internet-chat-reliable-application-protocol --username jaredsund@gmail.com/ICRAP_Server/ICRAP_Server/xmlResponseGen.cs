using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace ICRAP_Server
{
    class xmlResponseGen
    {
        XmlDocument doc;
        XmlElement root;

        public xmlResponseGen()
        {
            doc = new XmlDocument();// Create the XML Declaration, and append it to XML document
            XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "utf-8", null);
            doc.AppendChild(dec);// Create the root element
            root = doc.CreateElement("Response");
        }

        public string response(string status)
        {
            root.SetAttribute("status", status);
            root.SetAttribute("comment", "");
            root.SetAttribute("data", "");
            doc.AppendChild(root);

            return doc.InnerXml.ToString();
        }

        public string response(string status, string comment)
        {
            root.SetAttribute("status", status);
            root.SetAttribute("comment", comment);
            root.SetAttribute("data", "");
            doc.AppendChild(root);

            return doc.InnerXml.ToString();
        }

        public string response(string status, string comment, string data)
        {
            root.SetAttribute("status", status);
            root.SetAttribute("comment", comment);
            root.SetAttribute("data", data);
            doc.AppendChild(root);

            return doc.InnerXml.ToString();
        }


    }
}
