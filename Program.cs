using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SocketConnection
{
    class Program
    {
        static void Main(string[] args)
        {
            SocketServer ss = new SocketServer();
            ss.cmd_callback += (body_data) =>
                {
                    Console.WriteLine(body_data);
                };
            ss.Start(5956);

            Thread.Sleep(1000);

            SocketClient sc = new SocketClient();
            sc.Start("192.168.0.113", 5956);

            Thread.Sleep(1000);
            string json = "fghfdhfdhgfhdfhfgh";


            while (true)
            {
                sc.SendPackage(json);

                Thread.Sleep(1000);
            }

        }
    }
}
