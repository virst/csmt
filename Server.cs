using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace csmt
{
    class Server
    {
        private TcpListener Listener;

        public Server(int port)
        {
            Listener = new TcpListener(port);
        }

        public void Start()
        {
            Listener.Start();
            Console.WriteLine("Server Start");
           // Listen();
        }
        ~Server()
        {
            if (Listener != null)
                Listener.Stop();
        }
        /// <summary>
        /// Ждём подключений к нашему серверу и обрабатываем их
        /// </summary>
        public void Listen()
        {
            Console.WriteLine("Server Listen");
            while (true)
            {
                var client = Listener.AcceptTcpClient();
                Interaction.Interact(client);

            }
        }
    }
}
