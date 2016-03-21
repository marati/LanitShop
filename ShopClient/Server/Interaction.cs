﻿using System;
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
        TcpClient _tcpClient;

        public Interaction(int port)
        {
            _serverPort = port;
            CreateListener(Properties.Settings.Default.clientPort);
        }

        ~Interaction()
        {
            using (_tcpClient) { };

            try
            {
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

        bool ConnectToServer()
        {
            _tcpClient = new TcpClient();
            
            try
            {
                _tcpClient.Connect(_ipAddress, _serverPort);
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.ToString());
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
