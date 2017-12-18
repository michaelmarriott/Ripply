using System;
using System.Collections.Generic;
using System.Text;
using HtmlAgilityPack;
using Ripply.Models;

namespace Ripply
{
    public class HtmlResponse : IResponse
    {
        private readonly HtmlDocument _htmlDocument;
        public string Url { get; }

        /// <summary>
        /// Each page found executes to this method
        /// </summary>
        /// <param name="htmlDocument">An HtmlDocument of the response</param>
        /// <param name="url">Url of the page</param>
        public HtmlResponse(HtmlDocument htmlDocument, string url)
        {
            _htmlDocument = htmlDocument;
            Url = url;
        }

      
        
        public string Meta(string name)
        {
            if (this._htmlDocument.DocumentNode.SelectSingleNode($"//meta[@itemprop='{name}']") != null)
            {
                return this._htmlDocument.DocumentNode.SelectSingleNode($"//meta[@itemprop='{name}']").Attributes["content"].Value;
            }
            return this._htmlDocument.DocumentNode.SelectSingleNode($"//meta[@name='{name}']").Attributes["content"].Value;
        }

        public HtmlNodeCollection Css(string type, string property, string name)
        {

            return this._htmlDocument.DocumentNode.SelectNodes($"//{type}[@{property}='{name}']");
        }

        public override string ToString()
        {
            return _htmlDocument.DocumentNode.InnerHtml;
        }
    }
}
