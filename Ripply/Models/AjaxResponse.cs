using System;
using System.Collections.Generic;
using System.Text;

namespace Ripply.Models
{
    public class AjaxResponse : IResponse
    {

        public string Url { get; }

        public string Request { get; }

        public string Content { get;  }
        


        public AjaxResponse(string content, string url, string request)
        {
            Content = content;
            Url = url;
            Request = request;
        }

    }
}
