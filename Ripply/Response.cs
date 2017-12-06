using System;
using System.Collections.Generic;
using System.Text;
using HtmlAgilityPack;

namespace Ripply
{
    public class Response
    {
        private readonly HtmlDocument _htmlDocument;

        /// <summary>
        /// Each page found executes to this method
        /// </summary>
        /// <param name="htmlDocument">An HtmlDocument of the response</param>
        /// <param name="url">Url of the page</param>
        public Response(HtmlDocument htmlDocument, string url)
        {
            _htmlDocument = htmlDocument;
            Url = url;
        }

        public string Url { get; }
        
        /// <summary>
        /// Gets the meta tag value
        /// </summary>
        /// <param name="name">Name of the meta tag</param>
        /// <returns></returns>
        public string Meta(string name)
        {
            if (this._htmlDocument.DocumentNode.SelectSingleNode($"//meta[@itemprop='{name}']") != null)
            {
                return this._htmlDocument.DocumentNode.SelectSingleNode($"//meta[@itemprop='{name}']").Attributes["content"].Value;
            }
            return this._htmlDocument.DocumentNode.SelectSingleNode($"//meta[@name='{name}']").Attributes["content"].Value;
        }

        /// <summary>
        /// Gets the HtmlNodeCollection
        /// </summary>
        /// <param name="element">The element to find (EG:div,spa,td...)</param>
        /// <param name="attribute">The attribute to find (EG:style,class,id,...)</param>
        /// <param name="value">The value of the attribute </param>
        /// <returns></returns>
        public HtmlNodeCollection Css(string element, string attribute, string value)
        {
            return this._htmlDocument.DocumentNode.SelectNodes($"//{element}[@{attribute}='{value}']");
        }

        public override string ToString()
        {
            return _htmlDocument.DocumentNode.InnerText;
        }
    }
}
