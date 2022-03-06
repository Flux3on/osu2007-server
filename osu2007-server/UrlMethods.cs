using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;

namespace osu2007server
{
    public static class UrlMethods
    {
        public static byte[] Login(HttpListenerRequest r)
        {
            string username = r.QueryString["username"];
            string password = r.QueryString["password"];

            return Encoding.UTF8.GetBytes("1");
        }

        public static byte[] GetScores(HttpListenerRequest r)
        {
            string mapHash = r.QueryString["c"];
            return Encoding.UTF8.GetBytes("0:Flux:500000:600:0:15:575:0:0:0:True:0\n0:Not Flux:420727:444:0:15:575:13213:0:0:True:4");
        }

        public static byte[] GetUserInfo(HttpListenerRequest r)
        {
            string username = r.QueryString["username"];
            return Encoding.UTF8.GetBytes($"{username}|9|1111|-1|97.27|1");
        }

        public static byte[] SubmitScore(HttpListenerRequest r)
        {
            string score_line = r.QueryString["score"];
            string password = r.QueryString["password"];

            int len = int.Parse(r.Headers["Content-Length"]);
            byte[] buffer = new byte[len];
            r.InputStream.Read(buffer, 0, len);

            string stringBuffer = Encoding.UTF8.GetString(buffer);
            List<byte> bytes = new List<byte>(buffer);
            string[] splitString = stringBuffer.Split('\n');
            int lengthOfFourLines = splitString[0].Length + splitString[1].Length +
            splitString[2].Length + splitString[3].Length + 4;
            bytes.RemoveRange(0, lengthOfFourLines);
            int lengthOfLastLine = splitString[^2].Length + 2;
            bytes.RemoveRange(bytes.Count - lengthOfLastLine, lengthOfLastLine);
            buffer = bytes.ToArray();

            File.WriteAllBytes("something.osr", buffer);

            return new byte[0];
        }

        public static byte[] GetReplay(HttpListenerRequest r)
        {
            int id = int.Parse(r.QueryString["c"]);
            return File.ReadAllBytes("something.osr");
        }

    }
}
