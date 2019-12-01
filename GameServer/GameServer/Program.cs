using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;

namespace GameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server(IPAddress.Any, 8890);
            server.Start();
            while (true)
            {
                Console.ReadKey();
            }
        }

    }
}
