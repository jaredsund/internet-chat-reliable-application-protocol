using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data;

namespace TCP_Client_Test
{
    class xmlControllerParser
    {
        XmlDocument doc;
        XmlNode node;

        public xmlControllerParser(string inputString)
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

        public void channels(ref DataTable dt)
        {
            dt = new DataTable("Channels");
            dt.Columns.Add(new DataColumn("displayname", typeof(string)));
            dt.Columns.Add(new DataColumn("name", typeof(string)));
            dt.Columns.Add(new DataColumn("port", typeof(int)));
            dt.Columns.Add(new DataColumn("NoUsers", typeof(int)));
            //dt.AcceptChanges();

            XmlNodeList  channelNodes = doc.SelectNodes(@"//channel");
            foreach (XmlNode channel in channelNodes)
            {
                DataRow dr = dt.NewRow();
                dr["name"] = channel.Attributes["name"].Value.ToString();
                dr["port"] = int.Parse ( channel.Attributes["port"].Value);
                dr["noUsers"] = int.Parse(channel.Attributes["noUsers"].Value);
                dr["displayname"] = dr["name"] + "(" + dr["noUsers"].ToString () + ")";
                dt.Rows.Add(dr);
            }//end for each channel loop

            dt.AcceptChanges();
        }
    }
}
