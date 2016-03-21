using System;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Xml;
using System.Xml.Serialization;

namespace ShopClient
{
    class Helper
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

        public static object DeserializeXml(Type type, String filePath)
        {
            object result = null;
            XmlReader reader = null;

            try
            {
                reader = XmlReader.Create(filePath);
            }
            catch (System.IO.FileNotFoundException)
            {
                Console.WriteLine("Принятый файл не обнаружен по пути {0}", filePath);
            }
            catch (System.Security.SecurityException e)
            {
                Console.WriteLine("Не удалось обработать принятый файл. Причина:\n{0}", e.ToString());
            }

            if (reader != null)
            {
                XmlSerializer serializer = new XmlSerializer(type);

                try
                {
                    result = serializer.Deserialize(reader);
                    Console.WriteLine("Присланное сообщение с типом {0} успешно десериализовано.", type.ToString());
                }
                catch (InvalidOperationException)
                {
                    Console.WriteLine("Не удалось десериализовать присланное сообщение с типом {0}", type.ToString());
                }

                reader.Close();
            }

            return result;
        }
    }
}
