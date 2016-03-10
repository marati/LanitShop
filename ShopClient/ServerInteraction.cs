using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Xml.Serialization;

namespace ShopClient
{
    class ServerInteraction
    {
        String _host;
        int _port;
        TcpClient _tcpClient = new TcpClient();

        public ServerInteraction(String host, int port)
        {
            _host = host;
            _port = port;
        }

        ~ServerInteraction()
        {
            using (_tcpClient) { };
        }

        bool ConnectToServer()
        {
            if (!_tcpClient.Connected)
            {
                try
                {
                    _tcpClient.Connect(_host, _port);
                }
                catch (SocketException e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            return _tcpClient.Connected;
        }

        //TODO IEnumerable , yield return
        public void ReceiveXmlMessages(int listenerPort)
        {
            try
            {
                TcpListener listener = new TcpListener(Helper.GetIpAddress("wireless"), listenerPort);
                listener.Start();

                while (true)
                {
                    Console.WriteLine("Ожидание сообщения от клиента..");
                    TcpClient client = listener.AcceptTcpClient();

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

                        //yield return fileName
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

        public bool SendMessage(object message)
        {
            bool isSend = false;

            if (ConnectToServer())
            {
                var xmlSerializer = new XmlSerializer(message.GetType());

                using (NetworkStream networkStream = _tcpClient.GetStream())
                    if (networkStream.CanWrite)
                        xmlSerializer.Serialize(networkStream, message);

                isSend = true;
            }

            return isSend;
        }
    }
}
