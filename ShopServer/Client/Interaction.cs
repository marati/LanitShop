using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace ShopServer.Client
{
    class Interaction
    {
        TcpListener _tcpListener;
        Command _command = new Command();
        Dictionary<int, IPEndPoint> _shopOfClient = new Dictionary<int, IPEndPoint>();

        public Interaction(int port)
        {
            _tcpListener = new TcpListener(Helper.GetIpAddress("wireless"), port);
        }
        
        ~Interaction()
        {
            try
            {
                _tcpListener.Stop();
            }
            catch (SocketException)
            {
            }
        }

        static bool SendResponse(object message, NetworkStream clientStream)
        {
            bool isSend = false;

            try
            {
                var xmlSerializer = new XmlSerializer(message.GetType());

                using (clientStream)
                    if (clientStream.CanWrite)
                        xmlSerializer.Serialize(clientStream, message);

                isSend = true;
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.ToString());
            }

            return isSend;
        }

        bool NotifyClient(int shopId, IPEndPoint endPoint, object response)
        {
            bool isNotified = false;

            try
            {
                TcpClient client = new TcpClient();
                client.Connect(endPoint);

                if (SendResponse(response, client.GetStream()))
                {
                    isNotified = true;
                    Console.WriteLine(
                        "Клиенту {0}:{1} (shop id: {2}) успешно отправлено оповещение.",
                        endPoint.Address.ToString(), endPoint.Port, shopId
                    );
                }
            }
            catch (SocketException)
            {
                Console.WriteLine("Не удалось подключиться к клиенту с Ip {0}", endPoint.Address.ToString());
            }

            return isNotified;
        }

        void NotifyClients(object response)
        {
            List<int> inaccessibleClients = new List<int>();

            foreach (KeyValuePair<int, IPEndPoint> shopIp in _shopOfClient)
                if (NotifyClient(shopIp.Key, shopIp.Value, response))
                    inaccessibleClients.Add(shopIp.Key);

            foreach (int shopId in inaccessibleClients)
                if (_shopOfClient.Remove(shopId))
                    Console.WriteLine("Магазин с Id {0} удалён из списка отправки оповещений.", shopId);
        }

        void ProcessShopEntity(XmlReader reader, IPAddress clientIp)
        {
            ShopEntity data = (ShopEntity)Helper.DeserializeXml(typeof(ShopEntity), reader);

            if (data != null)
            {
                int shopId = _command.InsertShop(data);
                IPEndPoint endPoint = new IPEndPoint(clientIp, data.Port);

                if (_shopOfClient.ContainsKey(shopId))
                {
                    _shopOfClient[shopId] = endPoint;
                }
                else
                {
                    _shopOfClient.Add(shopId, endPoint);

                    object response = _command.ShopEntityResponse(data.Token, shopId);
                    if (response != null)
                        NotifyClient(shopId, endPoint, response);
                }
            }
        }

        void ProcessShopInfo(XmlReader reader)
        {
            object info = Helper.DeserializeXml(typeof(ShopInfo), reader);

            if (info != null)
            {
                ShopInfo data = (ShopInfo)info;
                object response = _command.GetShopById(data.Id);
                if (response != null)
                {
                    //TODO: получать данные для endPoint (адрес, порт) из БД
                    IPEndPoint endPoint = new IPEndPoint();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootNode"></param>
        /// <param name="reader"></param>
        /// <returns>Ответ клиенту</returns>
        void HandleCommand(String rootNode, XmlReader reader, IPAddress clientIp)
        {
            switch (rootNode)
            {
                case "ShopEntity":
                    ProcessShopEntity(reader, clientIp);
                    break;

                case "ShopInfo":
                    ProcessShopInfo(reader);
                    break;

                case "GoodData":
                    break;
            }
        }

        void ProcessRequest(String path, IPAddress clientIp)
        {
            try
            {
                using (XmlReader reader = XmlReader.Create(path))
                {
                    String rootNode = Helper.RetriveRootNode(reader);

                    if (rootNode == null)
                    {
                        Console.WriteLine(
                            "В принятом файле {0} не обнаружено ни одного элемента, обработка команды производится не будет.",
                            path
                        );
                    }
                    else
                    {
                        HandleCommand(rootNode, reader, clientIp);
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Принятый файл не обнаружен по пути {0}", path);
            }
            catch (System.Security.SecurityException e)
            {
                Console.WriteLine("Не удалось обработать принятый файл. Причина:\n{0}", e.ToString());
            }
        }

        public void ReceiveMessages()
        {
            try
            {
                _tcpListener.Start();

                while (true)
                {
                    Console.WriteLine("Ожидание сообщения от клиента..");
                    TcpClient client = _tcpListener.AcceptTcpClient();

                    IPEndPoint ipEndPoint = (IPEndPoint)client.Client.RemoteEndPoint;
                    String remoteIp = ipEndPoint.Address.ToString();
                    Console.WriteLine("Прислано новое сообщение с IP: {0}", remoteIp);

                    try
                    {
                        String receivedFileName = client.GetHashCode() + "-message.xml";

                        using (NetworkStream clientStream = client.GetStream())
                        using (StreamReader reader = new StreamReader(clientStream))
                        using (StreamWriter writer = new StreamWriter(receivedFileName))
                        {
                            String line;

                            while ((line = reader.ReadLine()) != null)
                                writer.WriteLine(line);

                            Console.WriteLine("Сообщение записано в файл {0}", receivedFileName);
                        }

                        ProcessRequest(receivedFileName, ipEndPoint.Address);
                    }
                    catch (InvalidOperationException e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }

            }
            catch (SocketException e)
            {
                Console.WriteLine(e.ToString());
            }
        }

    }
}
