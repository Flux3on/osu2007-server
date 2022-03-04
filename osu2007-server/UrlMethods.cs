using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osu2007server
{
    public static class UrlMethods
    {
        public static byte[] Login(string username, string password)
        {
            return Encoding.UTF8.GetBytes("1");
        }

        public static byte[] GetScores(string mapHash)
        {
            return Encoding.UTF8.GetBytes("0:Flux:500000:600:0:15:575:0:0:0:True:0\n0:Not Flux:420727:444:0:15:575:13213:0:0:True:4");
        }

        public static byte[] GetUserInfo(string username)
        {
            return Encoding.UTF8.GetBytes($"{username}|9|1111|-1|97.27|1");
        }

    }
}
