using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Ripply
{
    public static class EngineHelper
    {
        public static string EnsureCompleteUrl(string site, string link)
        {
            if (link.StartsWith("http://") || link.StartsWith("https://"))
            {
                return link;
            }
            return site + link;
        }


        public static void CheckStackTrace()
        {
            StackTrace s = new StackTrace();
            Console.WriteLine($"StackTrace {s.FrameCount}");
        }
    }
}
