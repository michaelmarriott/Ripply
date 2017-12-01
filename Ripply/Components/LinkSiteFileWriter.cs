using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Ripply.Components
{
    public class LinkSiteFileWriter : ILinkSite
    {
        public void Create(string store, string href, bool isItem)
        {
            File.AppendAllText($"{store}.csv",$"{href},{isItem}{Environment.NewLine}");
        }
    }
}
