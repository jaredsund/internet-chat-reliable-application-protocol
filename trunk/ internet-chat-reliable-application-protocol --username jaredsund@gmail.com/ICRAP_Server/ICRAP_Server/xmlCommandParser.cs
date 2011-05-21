using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace ICRAP_Server
{
    class xmlCommandParser
    {
        XmlDocument doc;
        XmlNode node;
        

        public xmlCommandParser(string inputString)
        {
            doc = new XmlDocument();
            doc.InnerXml = inputString;
            node = doc.SelectSingleNode("Request");
        }

        public string command
        {
            get { return node.Attributes["command"].Value; }
        }

        public string clientName
        {
            get { return node.Attributes["clientname"].Value; }
        }

        public string data
        {
            get { return node.Attributes["data"].Value; }
        }

    }
}
