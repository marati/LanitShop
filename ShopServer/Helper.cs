﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Xml;

namespace ShopServer
{
    static class Helper
    {
        static NetworkInterfaceType GetInterfaceByString(String type)
        {
            NetworkInterfaceType interfaceType = NetworkInterfaceType.Ethernet;

            switch (type.ToLower())
            {
                case "ethernet":
                    interfaceType = NetworkInterfaceType.Ethernet;
                    break;

                case "wireless":
                    interfaceType = NetworkInterfaceType.Wireless80211;
                    break;
            }

            return interfaceType;
        }

        public static IPAddress GetIpAddress(String interfaceType)
        {
            IPAddress resultIp = IPAddress.Parse("127.0.0.1");
            bool stopSearch = false;

            try
            {
                foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (item.NetworkInterfaceType == GetInterfaceByString(interfaceType) &&
                        item.OperationalStatus == OperationalStatus.Up)
                    {
                        foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                            if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                resultIp = ip.Address;
                                stopSearch = true;
                                break;
                            }
                    }

                    if (stopSearch)
                        break;
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("Не удалось получить ip адрес: {0}", e.ToString());
            }

            return resultIp;
        }

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