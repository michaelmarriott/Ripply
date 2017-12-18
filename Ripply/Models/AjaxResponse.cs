using System;
using System.Collections.Generic;
using System.Text;

namespace Ripply.Models
{
    public class AjaxResponse : IResponse
    {

        public string Url { get; }

        public dynamic Request { get; }

        public dynamic Content { get;  }


        public AjaxResponse(dynamic content, string url, dynamic request)
        {
            Content = content;
            Url = url;
            Request = request;
        }

    }
}
