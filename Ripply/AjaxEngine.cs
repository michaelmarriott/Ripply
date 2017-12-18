using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net.Http;
using Ripply.Models;

namespace Ripply
{
    public class AjaxEngine
    {
        private static readonly Logger Log = new LoggerConfiguration().CreateLogger();
        private AjaxScrapper _scrapper;
        private Stopwatch _sw;

        public AjaxEngine(AjaxScrapper scrapper)
        {
            _scrapper = scrapper;
        }

        public async Task CrawlAsync()
        {
            await Execute(_scrapper.BaseUrl);
        }

        private async Task Execute(string url)
        {
            try
            {
                _sw = Stopwatch.StartNew();
                string responseContent;
                using (var client = new HttpClient())
                {
                    foreach(var item in _scrapper.Request.Headers)
                    {
                        client.DefaultRequestHeaders.Add(item.Key, item.Value);
                    }
                    
                    HttpResponseMessage response = await client.GetAsync(url);
                    var body = _scrapper.GetBody();
                    if (body == null)
                        return;
                    responseContent = await response.Content.ReadAsStringAsync();
                    await _scrapper.Process(new AjaxResponse(responseContent, url, body ));
                }
                _sw.Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

    }
}
