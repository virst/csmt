using System;

namespace csmt
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            int port = 1234;
            if (args.Length > 0)
                int.TryParse(args[0], out port);
            Server s = new Server(port);
            Console.WriteLine("Port = " + port);
            s.Start();
            s.Listen();
        }
    }
}
