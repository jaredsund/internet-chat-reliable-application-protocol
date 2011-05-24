using System;
using System.Collections;
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
        XmlDeclaration dec;

        public xmlResponseGen()
        {
            setupRoot();
            
        }

        private void setupRoot()
        {
            doc = new XmlDocument();// Create the XML Declaration, and append it to XML document
            dec = doc.CreateXmlDeclaration("1.0", "utf-8", null);
            doc.AppendChild(dec);// Create the root element
            root = doc.CreateElement("Response");
        }

        private string response(string status, string comment, string data)
        {
            setupRoot();
            root.SetAttribute("status", status);
            root.SetAttribute("comment", comment);
            root.SetAttribute("data", data);
            doc.AppendChild(root);

            return doc.InnerXml.ToString();
        }

       

        public string sResponse()
        {
            return response("Sucess", "", "");
        }

        public string sResponse(string comment)
        {
            return response("Sucess", comment, "");
        }

        public string sResponse(string comment, string data)
        {
            return response("Sucess", comment, data);
        }

        public string responseEnumChannels(ref ArrayList channels)
        {
            setupRoot();
            root.SetAttribute("status", "Success");
            root.SetAttribute("comment", "");

            XmlElement data = doc.CreateElement("data");

            foreach (ChannelThread cT in channels)
            {
                XmlElement channel = doc.CreateElement("channel");
                channel.SetAttribute("name", cT.Name);
                channel.SetAttribute("port", cT.PortNo.ToString());
                channel.SetAttribute("noUsers", cT.numUsers.ToString());
                data.AppendChild(channel);
            }

            root.AppendChild(data);

            doc.AppendChild(root);

            return doc.InnerXml.ToString();
        }

        public string eResponse(string comment)
        {
            return response("Error", comment, "");
        }

        public string clientResponse(string command, string data)
        {
            setupRoot();
            root.SetAttribute("command", command);
            root.SetAttribute("data", data);
            doc.AppendChild(root);

            return doc.InnerXml.ToString();
        }

        public string responseEnumClients(ref ArrayList clients)
        {
            setupRoot();
            root.SetAttribute("status", "Success");
            root.SetAttribute("comment", "");

            XmlElement data = doc.CreateElement("data");

            foreach (ChannelClient  cL in clients)
            {
                XmlElement channel = doc.CreateElement("channel");
                channel.SetAttribute("name", cL.username );
                channel.SetAttribute("id", cL.id );
                data.AppendChild(channel);
            }

            root.AppendChild(data);

            doc.AppendChild(root);

            return doc.InnerXml.ToString();
        }
    }
}
