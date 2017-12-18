using System;
using System.Collections.Generic;
using System.Text;

namespace Ripply.Models
{
    public class Request
    {
        private Dictionary<string, string> headers;

        public Dictionary<string, string> Headers
        {
            get { return headers; }
            set
            {
                if(headers == null)
                {
                    headers = new Dictionary<string, string>();
                }

            }
        }
        public string Method { get; set; } = "GET";
    }
}
