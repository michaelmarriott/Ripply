using System;
using HtmlAgilityPack;

namespace Ripply.Extensions
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
