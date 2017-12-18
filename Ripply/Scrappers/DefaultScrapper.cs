using Ripply.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ripply
{
    public abstract class DefaultScrapper
    {

        public Request Request;
        public string SiteName;

        public DefaultScrapper()
        {
            this.Request = new Request();
        }

     

        public string DefaultFileName()
        {
            return $"{SiteName}.csv";
        }


    }
}
