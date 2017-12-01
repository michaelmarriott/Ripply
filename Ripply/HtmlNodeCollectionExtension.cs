using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace Ripply
{
    public static class HtmlNodeCollectionExtension
    {

      

        public static HtmlNode ExtractFirst(this HtmlNodeCollection node)
        {

            return node[0];
        }



        public static string ToString(this HtmlNode node)
        {
            return node.InnerText;
        }

    }
}
