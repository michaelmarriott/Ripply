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

        public string[] ValidPageUrlParts { get; set; }

        public string[] InValidPageUrlParts { get; set; }

        public string[] ItemPageUrlParts { get; set; }

        public string[] QueryStringPartRemove { get; set; }

        public string[] QueryStringIncludeOnly { get; set; }

        public virtual async Task Process(Response response)
        {
            throw new NotImplementedException();
        }
    }
}
