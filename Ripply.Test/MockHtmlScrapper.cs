using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ripply.Test
{
    public class MockHtmlScrapper : HtmlScrapper
    {
        public MockHtmlScrapper()
        {
            this.SiteName = "Mock";

            this.StartingUrl = "https://testsite";
            this.UrlContainsToFollow = new[] { "/store1/", "/store2/", "/store3/" };
            this.UrlContainsToDontFollow = new[] { "dontfollow" };
            this.UrlContainsItem = new[] { "/item" };
            this.QueryStringRemove = new[] { "extraquerystringinfo" };
        }


        public override async Task Process(HtmlResponse response)
        {
        }
    }
}
