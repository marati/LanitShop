using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace ShopServer
{
    class Program
    {
        //Первый параметр - port
        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                int port;

                if (!Int32.TryParse(args[0], out port))
                {
                    Console.WriteLine("Не удалось распознать порт ({0}), проверьте ввод аргументов", args[0]);
                    Environment.Exit(0);
                }

                new ClientInteraction().ReceiveMessages(port);
            }
            else
            {
                Console.WriteLine("Неправильный ввод аргументов. Пример: ShopServer.exe 1111");
            }
        }
    }
}
