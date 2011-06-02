using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data;

namespace TCP_Client_Test
{
    class xmlChannelParser
    {
         XmlDocument doc;
        XmlNode node;

        public xmlChannelParser(string inputString)
        {
            doc = new XmlDocument();
            doc.InnerXml = inputString;
            node = doc.SelectSingleNode("Response");
        }

        public string status
        {
            get { return node.Attributes["status"].Value; }
        }

        public string data
        {
            get { return node.Attributes["data"].Value; }
        }

        public string command
        {
            get { return node.Attributes["command"].Value; }
        }

        
    }
}
