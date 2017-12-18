using Ripply.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace Ripply
{
    public abstract class HtmlScrapper : DefaultScrapper
    {

        public string StartingUrl;

        public string[] UrlContainsToFollow { get; set; }

        public string[] UrlContainsToDontFollow { get; set; }

        public string[] UrlContainsItem { get; set; }

        public string[] QueryStringRemove { get; set; }

        public string[] QueryStringIncludeOnly { get; set; }

        public HtmlScrapper() : base()
        {
        }

        public virtual async Task Process(HtmlResponse response)
        {
            throw new NotImplementedException();
        }

        public string GetBaseUrl()
        {
            return Regex.Match(StartingUrl, @"http[s]?:\/\/?([\da-z\.-]+)/", RegexOptions.Singleline).ToString();
        }

    }
}
