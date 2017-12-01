using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Ripply
{
    public abstract class IScrapper
    {
        public string SiteName;

        public string StartingUrl;

         internal bool NewOnly;

        public string[] ValidLinks { get; set; }

        public string[] InValidLinks { get; set; }

        public string[] ItemPage { get; set; }

        public string[] QueryStringIgnore { get; set; }

        public string[] QueryStringIncludeOnly { get; set; }

        public virtual async Task Process(Response response)
        {
            throw new NotImplementedException();
        }
    }
}
