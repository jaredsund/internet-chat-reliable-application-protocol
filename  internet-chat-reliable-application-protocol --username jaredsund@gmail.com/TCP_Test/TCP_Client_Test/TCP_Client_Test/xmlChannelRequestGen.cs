﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace TCP_Client_Test
{
    class xmlChannelRequestGen
    {
        XmlDocument doc;
        XmlElement root;

        public xmlChannelRequestGen()
        {
            setupRoot();
        }

        public string postMessage(string message)
        {
            setupRoot();
            root.SetAttribute("command", "PostMessage");
            root.SetAttribute("clienname", "");
            root.SetAttribute("data", message);
            doc.AppendChild(root);

            return doc.InnerXml.ToString();
        }

        private void setupRoot()
        {
            doc = new XmlDocument();// Create the XML Declaration, and append it to XML document
            XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "utf-8", null);
            doc.AppendChild(dec);// Create the root element
            root = doc.CreateElement("Request");
        }

       
    }
}
