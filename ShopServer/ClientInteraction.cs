using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace ShopServer
{
    class ClientInteraction
    {
        Command _command = new Command();

        void SaveClientData(NetworkStream clientStream)
        {
            
        }

        static void SendResponse(object message, NetworkStream clientStream)
        {
            try
            {
                var xmlSerializer = new XmlSerializer(message.GetType());

                using (clientStream)
                    if (clientStream.CanWrite)
                        xmlSerializer.Serialize(clientStream, message);

                Console.WriteLine("Отправлен ответ на запрос клиента.");
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootNode"></param>
        /// <param name="reader"></param>
        /// <returns>Ответ клиенту</returns>
        object HandleCommand(String rootNode, XmlReader reader)
        {
            object response = null;

            switch (rootNode)
            {
                case "ShopData":
                    XmlSerializer serializer = new XmlSerializer(typeof(ShopData));
                    ShopData data = (ShopData)serializer.Deserialize(reader);
                    int shopId = _command.InsertShop(data);

                    response = _command.CreateResponse(data.Token, shopId);
                    break;

                case "GoodData":
                    break;
            }

            return response;
        }

        void ProcessFile(String path, NetworkStream clientStream)
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
                    object response = HandleCommand(rootNode, reader);
                    SendResponse(response, clientStream);
                }
            }
        }

        public void ReceiveMessages(int port)
        {
            try
            {
                TcpListener listener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
                listener.Start();

                while (true)
                {
                    Console.WriteLine("Ожидание сообщения от клиента..");
                    TcpClient client = listener.AcceptTcpClient();
                    String remoteIp = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                    Console.WriteLine("Прислано новое сообщение с IP: {0}", remoteIp);

                    try
                    {
                        String receivedFileName = client.GetHashCode() + "-message.xml";

                        /*using*/
                        //TODO: контроль за освобождением этого ресурса
                        NetworkStream clientStream = client.GetStream();
                        using (StreamReader reader = new StreamReader(clientStream))
                        using (StreamWriter writer = new StreamWriter(receivedFileName))
                        {
                            String line;

                            while ((line = reader.ReadLine()) != null)
                                writer.WriteLine(line);

                            Console.WriteLine("Сообщение записано в файл {0}", receivedFileName);
                        }

                        ProcessFile(receivedFileName, clientStream);
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
