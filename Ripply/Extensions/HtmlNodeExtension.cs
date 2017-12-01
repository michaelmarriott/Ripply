using System;
using HtmlAgilityPack;

namespace Ripply.Extensions
{
    public static class HtmlNodeExtension
    {
        public static HtmlNodeCollection Css(this HtmlNode node, string type, string property, string name)
        {
            return node.SelectNodes($".//{type}[@{property}='{name}']");
        }

        public static HtmlNodeCollection Css(this HtmlNode node, string type)
        {
            return node.SelectNodes($".//{type}");
        }

        public static string ToString(this HtmlNode node)
        {
            return node.InnerText;
        }

    }
}
