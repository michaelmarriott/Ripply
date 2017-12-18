using Ripply.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ripply
{
    public class AjaxScrapper : DefaultScrapper
    {

        public string BaseUrl;

        public AjaxScrapper() : base()
        {
        }

        /// <summary>
        /// This method gets called to create the Body of the request message to be sent
        /// </summary>
        /// <returns></returns>
        public virtual string GetBody()
        {
            return null;
        }

        public virtual async Task Process(AjaxResponse response)
        {
            throw new NotImplementedException();
        }
       

    }
}
