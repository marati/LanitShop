using System;
using System.Xml;

namespace ShopServer
{
    class Helper
    {
        public static String RetriveRootNode(XmlReader reader)
        {
            String rootNodeName = null;

            while (reader.Read())
                if (reader.NodeType == XmlNodeType.Element)
                {
                    rootNodeName = reader.Name;
                    break;
                }

            return rootNodeName;
        }
    }
}
