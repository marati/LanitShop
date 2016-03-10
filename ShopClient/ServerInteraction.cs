using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
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

        //TODO IEnumerable , yield return
        void ReceiveXmlMessages()
        {
            try
            {
                using (NetworkStream clientStream = _tcpClient.GetStream())
                {
                    while (true)
                    {
                        using (StreamReader reader = new StreamReader(clientStream))
                        {
                            String line;

                            while ((line = reader.ReadLine()) != null)
                            {
                                Console.WriteLine("Received: " + line);
                            }
                        }

                        Thread.Sleep(10000);
                    }
                }
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine(e.ToString());
            }

        }

        public bool SendMessage(object message)
        {
            bool isSend = false;

            try
            {
                _tcpClient.Connect(_host, _port);
                var xmlSerializer = new XmlSerializer(message.GetType());
                
                using (var networkStream = _tcpClient.GetStream())
                    if (networkStream.CanWrite)
                        xmlSerializer.Serialize(networkStream, message);

                isSend = true;
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.ToString());
            }

            return isSend;
        }

        
    }
}
