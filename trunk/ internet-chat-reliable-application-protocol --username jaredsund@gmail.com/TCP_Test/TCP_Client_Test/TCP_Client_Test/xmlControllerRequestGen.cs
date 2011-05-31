using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace TCP_Client_Test
{
    class xmlControllerRequestGen
    {
        XmlDocument doc;
        XmlElement root;
        private string userName;

        public xmlControllerRequestGen(string userName)
        {
            this.userName = userName;
            doc = new XmlDocument();// Create the XML Declaration, and append it to XML document
            XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "utf-8", null);
            doc.AppendChild(dec);// Create the root element
            root = doc.CreateElement("Request");
        }

        public string CreatChan(string channelName)
        {
            root.SetAttribute("command", "CreateChan");
            root.SetAttribute("clienname", userName);
            root.SetAttribute("data", channelName);
            doc.AppendChild(root);

            return doc.InnerXml.ToString();
        }

        public string EnumChan()
        {
            root.SetAttribute("command", "EnumChan");
            root.SetAttribute("clienname", userName);
            root.SetAttribute("data", "");
            doc.AppendChild(root);

            return doc.InnerXml.ToString();
        }
    }
}
