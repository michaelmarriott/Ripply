using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripply.Scrapper.Scrappers
{
    public class MyFirstScrapper : IScrapper
    {
        public MyFirstScrapper()
        {
            this.SiteName = "MyFirst";
            
            this.StartingUrl = "https://xxxxxxxxxxxx";
            this.ValidLinks = new [] { "" }; //This will only follow links that contain information specified in VlaidLinks
            this.ItemPage = new [] { "" }; //What does the url for the pages you wish find look like
            this.QueryStringIncludeOnly = new[] { "" }; //regex on querystring
        }


        public override async Task Process(Response response)
        {
			
		}
		
	}
	
}