using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace csmt
{
    class Interaction
    {
        static string Server_Directory = "C:\\Work\\HTTPServer\\page_content\\";

        static Interaction()
        {
            Server_Directory = Path.Combine(Environment.CurrentDirectory, "www");
        }

        public static void Interact(TcpClient client)
        {
            Thread t_interact = new Thread(() =>
            {
                Console.WriteLine("Interact");
                interact(client);
            });
            t_interact.Start();
        }

        static void interact(TcpClient client)
        {
            try
            {
                byte[] buffer = new byte[1024];
                string request = "";
                while (true)
                {
                    int count = client.GetStream().Read(buffer, 0, 1024);
                    request += Encoding.ASCII.GetString(buffer, 0, count);
                    if (request.IndexOf("\r\n\r\n") >= 0) // Запрос обрывается \r\n\r\n последовательностью
                    {
                        WriteToClient(client, request);
                        request = "";
                    }
                }
            }
            catch (Exception ex)
            {
                client.Close();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }

        /// <summary>
        ///  Отвечаем на запрос клиенту
        /// </summary>
        public static void WriteToClient(TcpClient client, string request)
        {
            Log.Write(request);

            var pp = GetPathParam(request);
            var file_path = pp.fn;
            if (file_path.EndsWith(".png") || file_path.EndsWith(".ico"))
            {
                FileStream file = File.Open(file_path.Replace("%20", " "), FileMode.Open);
                WriteHeaderToClient(client, "image/png", file.Length);
                byte[] buf = new byte[1024];
                int len;
                while ((len = file.Read(buf, 0, 1024)) != 0)
                    client.GetStream().Write(buf, 0, len);
                file.Close();
            }
            else if (file_path.EndsWith(".mp4"))
            {
                FileStream file = File.Open(file_path.Replace("%20", " "), FileMode.Open);
                WriteHeaderToClient(client, "video/mp4", file.Length);
                byte[] buf = new byte[1024];
                int len;
                while ((len = file.Read(buf, 0, 1024)) != 0)
                    client.GetStream().Write(buf, 0, len);
                file.Close();
            }
            else
            {
                var html = HtmlGenerator.GetIndexHtml(file_path, Server_Directory, pp.Parametrs["p1"]);
                var bb = Encoding.ASCII.GetBytes(html);
                WriteHeaderToClient(client, "text/html", bb.Length);
                client.GetStream().Write(bb, 0, bb.Length);
            }
        }

        static PathParam GetPathParam(string request)
        {
            int space1 = request.IndexOf(" ");
            int space2 = request.IndexOf(" ", space1 + 1);
            string url = request.Substring(space1 + 2, space2 - space1 - 2);
            Console.WriteLine("url " + url);
            return new PathParam(Path.Combine(Server_Directory, url.Replace('/', Path.DirectorySeparatorChar)));
        }

        /// <summary>
        /// Отправляем заголовок клиенту.
        /// </summary>
        public static void WriteHeaderToClient(TcpClient client, string content_type, long length)
        {
            string str = "HTTP/1.1 200 OK\nContent-type: " + content_type
                   + "\nContent-Encoding: 8bit\nContent-Length:" + length.ToString()
                   + "\n\n";
            client.GetStream().Write(Encoding.ASCII.GetBytes(str), 0, str.Length);
        }

        class PathParam
        {
            public PathParam(string s)
            {
                var ss = s.Split('?');
                fn = ss[0];
                if (ss.Length > 1)
                    prm = ss[1];
                //fn = fn.Replace("%20", " ");
            }

            public string fn = "";
            public string prm = "";

            public paramDic Parametrs
            {
                get
                {
                    paramDic p = new paramDic();
                    var ss = prm.Split('&');
                    foreach (var d in ss)
                    {
                        string v = null;
                        var ss2 = d.Split('=');
                        if (ss2.Length > 1)
                            v = ss2[1];
                        p.Add(ss2[0], v);
                    }

                    return p;
                }
            }

        }

        class paramDic : Dictionary<string, string>
        {
            public new string this[string s]
            {
                get
                {
                    if (base.ContainsKey(s))
                        return base[s];
                    return null;
                }

            }
        }
    }
}
