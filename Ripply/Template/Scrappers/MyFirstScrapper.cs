using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripply.Scrapper.Scrappers
{
    public class MyFirstScrapper : HtmlScrapper
    {
        public MyFirstScrapper() : base()
        {
            this.SiteName = "MyFirst";
            this.StartingUrl = "https://xxxxxxxxxxxx/xxx"; // Starting link for scrapper (https://www.firstsite.co.za/)
            this.UrlContainsToFollow = new [] { "" }; // Scrapper will only follow links that contain information specified ("/categories/","/products/")
            this.UrlContainsToDontFollow = new[] { "" }; //Scrapper will not follow links that contain information specified  ("/login")
            this.UrlContainsItem = new[] { "" }; // What does the url for the items you wish find look like ("/products/"), these items get passed to Process method
            this.QueryStringIncludeOnly = new[] { "" }; // Regex on querystring to ensure unique paging
            this.QueryStringRemove = new[] { "" }; // Regex on querystring to remove items from querystring to es=nsure unique page
        }
        
        public override async Task Process(HtmlResponse response)
        {
			Console.WriteLine($"Url:{response.Url}");
            Console.WriteLine($"Document String:{response.ToString()}");
            //response.Css("div", "class", "id");
            //response.Meta("name");
        }
    }
}