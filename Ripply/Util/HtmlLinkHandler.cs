using Ripply.Components;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Ripply.Util
{
    public class HtmlLinkHandler
    {
        private HtmlScrapper _scrapper;
        public HtmlLinkHandler(HtmlScrapper scrapper)
        {
            _scrapper = scrapper;
        }

        public bool IsValidLink(string link)
        {
            if (_scrapper.UrlContainsToDontFollow != null)
            {
                foreach (var invalidLink in _scrapper.UrlContainsToDontFollow)
                {
                    if (link.Contains(invalidLink))
                    {
                        return false;
                    }
                }
            }

            foreach (var validLink in _scrapper.UrlContainsToFollow)
            {
                if (link.Contains(validLink))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsItemPage(string link)
        {
            foreach (var itemPage in _scrapper.UrlContainsItem)
            {
                if (link.Contains(itemPage))
                {
                    return true;
                }
            }
            return false;
        }

        public string CleanQueryStringUrl(string url)
        {
            if (!url.Contains("?"))
            {
                return RemoveFromQueryString(url);
            }

            var path = url.Split("?")[0];
            var querysting = url.Split("?")[1];
            if (_scrapper.QueryStringIncludeOnly != null)
            {
                var tempQueryString = querysting;
                foreach (var item in _scrapper.QueryStringIncludeOnly)
                {
                    querysting = Regex.Match(tempQueryString, item, RegexOptions.Singleline).ToString();
                    if (!String.IsNullOrEmpty(querysting))
                        break;
                }
            }
            else
            {
                path = RemoveFromQueryString(url);
                querysting = RemoveFromQueryString(querysting);
            }
            return $"{path}?{querysting}";
        }


        public string RemoveFromQueryString(string querysting)
        {
            if (_scrapper.QueryStringRemove == null)
                return querysting;

            var tempQueryString = querysting;

            foreach (var item in _scrapper.QueryStringRemove)
            {
                var ignore = Regex.Match(tempQueryString, item, RegexOptions.Singleline).ToString();
                if (!String.IsNullOrEmpty(ignore))
                    querysting = querysting.Replace(ignore, "");
            }
            return querysting;
        }

    }
}
