using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Xml.Serialization;

namespace ShopClient
{
    class ServerInteraction
    {
        IPAddress _ipAddress = Helper.GetIpAddress("wireless");
        int _serverPort;

        TcpListener _tcpListener;
        TcpClient _tcpClient = new TcpClient();

        public ServerInteraction(int port)
        {
            _tcpListener = new TcpListener(_ipAddress, 0);
            _tcpListener.Start();
            _serverPort = port;
        }

        ~ServerInteraction()
        {
            using (_tcpClient) { };

            if (_tcpListener != null)
                _tcpListener.Stop();
        }

        public int GetListenerPort()
        {
            return ((IPEndPoint)_tcpListener.LocalEndpoint).Port;
        }

        bool ConnectToServer()
        {
            if (!_tcpClient.Connected)
            {
                try
                {
                    _tcpClient.Connect(_ipAddress, _serverPort);
                }
                catch (SocketException e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            return _tcpClient.Connected;
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

        //TODO IEnumerable , yield return
        public void ReceiveXmlMessages()
        {
            try
            {
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
    }
}
