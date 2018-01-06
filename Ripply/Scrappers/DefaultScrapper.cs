using Ripply.Models;

namespace Ripply
{
    public abstract class DefaultScrapper
    {

        public Request Request;
        public string SiteName;

        public DefaultScrapper()
        {
            this.Request = new Request();
            this.Request.Headers = new System.Collections.Generic.Dictionary<string, string>();
        }

     

        public string DefaultFileName()
        {
            return $"{SiteName}.csv";
        }


    }
}
