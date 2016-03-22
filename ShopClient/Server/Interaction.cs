using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Xml.Serialization;

namespace ShopClient.Server
{
    class Interaction
    {
        IPAddress _ipAddress = Helper.GetIpAddress("wireless");
        int _serverPort;

        TcpListener _tcpListener;

        public Interaction(int port)
        {
            _serverPort = port;
            CreateListener(Properties.Settings.Default.clientPort);
        }

        ~Interaction()
        {
            try
            {
                if (_tcpListener != null)
                    _tcpListener.Stop();
            }
            catch (SocketException)
            {
            }
        }

        public int GetListenerPort()
        {
            int port = 0;

            if (_tcpListener != null)
                port = ((IPEndPoint)_tcpListener.LocalEndpoint).Port;

            return port;
        }

        /// <summary>
        /// save to local settings
        /// </summary>
        /// <param name="port"></param>
        void SaveListenerPort(int listenerPort)
        {
            Properties.Settings.Default.clientPort = listenerPort;
            Properties.Settings.Default.Save();
        }

        void CreateListener(int port)
        {
            try
            {
                //if port == 0, listener generate random unused port
                _tcpListener = new TcpListener(_ipAddress, port);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.WriteLine("Не удалось создать listener, причина:\n{0}", e.ToString());
            }

            if (_tcpListener == null)
                Environment.Exit(0);

            try
            {
                _tcpListener.Start();
            }
            catch (SocketException)
            {
            }

            SaveListenerPort(((IPEndPoint)_tcpListener.LocalEndpoint).Port);
        }

        TcpClient ClientInstance()
        {
            TcpClient client = new TcpClient();
            
            try
            {
                client.Connect(_ipAddress, _serverPort);
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.ToString());
            }

            return client;
        }

        public bool SendMessage(object message)
        {
            bool isSend = false;

            using (TcpClient client = ClientInstance())
            {
                if (client.Connected)
                {
                    var xmlSerializer = new XmlSerializer(message.GetType());

                    try
                    {
                        using (NetworkStream clientStream = client.GetStream())
                        {
                            if (clientStream.CanWrite)
                            {
                                try
                                {
                                    xmlSerializer.Serialize(clientStream, message);
                                    isSend = true;
                                }
                                catch (InvalidOperationException e)
                                {
                                    Console.WriteLine("Не удалось сериализовать сообщение, отправка произведена не будет.");
                                    Console.WriteLine(e);
                                }
                            }
                        }
                    }
                    catch (ObjectDisposedException e)
                    {
                        Console.WriteLine("Невозможно получить доступ к сетевому потоку, возможно он был зыкрыт.");
                        Console.WriteLine(e.ToString());
                    }
                }
            }

            return isSend;
        }

        public IEnumerable<String> ReceiveXmlMessages()
        {
            while (true)
            {
                Console.WriteLine("Ожидание сообщения от сервера..");
                TcpClient client = null;

                try
                {
                    client = _tcpListener.AcceptTcpClient();

                    IPEndPoint ipEndPoint = (IPEndPoint)client.Client.RemoteEndPoint;
                    String remoteIp = ipEndPoint.Address.ToString();
                    Console.WriteLine("Прислано новое сообщение с IP: {0}", remoteIp);
                }
                catch (SocketException e)
                {
                    Console.WriteLine(e.ToString());
                }

                if (client == null)
                    break;

                String receivedFileName = null;

                try
                {
                    receivedFileName = client.GetHashCode() + "-message.xml";

                    //TODO: заменить на FileStream
                    using (NetworkStream clientStream = client.GetStream())
                    using (StreamReader reader = new StreamReader(clientStream))
                    using (StreamWriter writer = new StreamWriter(receivedFileName))
                    {
                        String line;

                        while ((line = reader.ReadLine()) != null)
                            writer.WriteLine(line);

                        Console.WriteLine("Сообщение записано в файл {0}", receivedFileName);
                    }
                }
                catch (InvalidOperationException e)
                {
                    Console.WriteLine(e.ToString());
                }

                yield return receivedFileName;
            }
        }
    }
}
