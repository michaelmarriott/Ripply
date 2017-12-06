using System;
using HtmlAgilityPack;

namespace Ripply.Extensions
{
    public static class HtmlNodeExtension
    {
        public static HtmlNodeCollection Css(this HtmlNode node,string element, string attribute, string value)
        {
            return node.SelectNodes($".//{element}[@{attribute}='{value}']");
        }

        public static HtmlNodeCollection Css(this HtmlNode node, string element)
        {
            return node.SelectNodes($".//{element}");
        }

        public static string ToString(this HtmlNode node)
        {
            return node.InnerText;
        }

    }
}
