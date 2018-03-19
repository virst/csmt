using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace csmt
{
    static class HtmlGenerator
    {
        public static string GetIndexHtml(string localAdr, string baseAdr, string video)
        {
            string html = string.Format(head, "Video", style1);
            html += "<table border=\"1\" style='height: 100%; width: 100%'>  <tr style= 'vertical-align: top;'> <td style = 'width: 300px'> ";

            if (localAdr.TrimEnd(Path.DirectorySeparatorChar) != baseAdr.TrimEnd(Path.DirectorySeparatorChar))
                html += string.Format(div, "..", "img/folder.png", "..");

            var dd = Directory.GetDirectories(localAdr);
            foreach (var d in dd)
            {
                var di = new DirectoryInfo(d);
                html += string.Format(div, "/" + di.Name, "img/folder.png", di.Name);
            }

            dd = Directory.GetFiles(localAdr, "*.mp4");
            foreach (var d in dd)
            {
                var di = new FileInfo(d);
                html += string.Format(div, "?p1=" + di.Name, "img/video.png", di.Name);
            }

            html += "</td> <td style='text-align:center;vertical-align:middle;'> " + VideoBlock(video) + " </td>  </tr>  </table> ";
            html += end;

            return html;
        }

        public static string VideoBlock(string file)
        {
            if (String.IsNullOrWhiteSpace(file))
                return @" <img src='img/play.png' />   ";
            return $"<video controls='controls' poster='img/camera.png'> <source src = '{file}' > </video> ";
        }

        static string UpLevel(string d1, string d2)
        {
            d1 = d1.TrimEnd(Path.DirectorySeparatorChar);
            d2 = d2.TrimEnd(Path.DirectorySeparatorChar);

            if (!d2.StartsWith(d1))
                return "";
            d2 = d2.Substring(d1.Length).TrimStart();
            string s = "";
            foreach (char o in d2)
            {
                if (o == Path.DirectorySeparatorChar)
                    s += "/..";
            }

            return s;
        }

        #region strings

        static string head = @"<html >
<head>   
    <title>{0}</title>
{1}
</head>
<body  >";

        static string end = @"</body>
</html>";

        static string div = "<div><a href='{0}'><img src='{1}' /> {2}</a></div>";

        static string style1 = @"<style>
        html, body, form, video {
            height: 100%;
            padding: 0px;
            margin: 0px;
        }
    </style>";

        #endregion
    }
}
