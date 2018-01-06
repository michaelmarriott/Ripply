using Ripply.Models;
using Serilog;
using Serilog.Core;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Ripply
{
    public class AjaxEngine
    {
        private static readonly Logger Log = new LoggerConfiguration().CreateLogger();
        private AjaxScrapper _scrapper;
        private Stopwatch _sw;
        private string _userAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/36.0.1985.125 Safari/537.36";

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
                using (var client = new HttpClient())
                {

                    foreach (var item in _scrapper.Request.Headers)
                    {
                        if (item.Key.Equals("content-type", StringComparison.CurrentCultureIgnoreCase))
                        {
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(item.Value));
                        }
                        else
                        {
                            client.DefaultRequestHeaders.Add(item.Key, item.Value);
                        }
                    }

                    var body = _scrapper.GetBodyRequest();
                    if (body == null)
                        return;
                    HttpResponseMessage response;
                    if (_scrapper.Request.Method.Equals("put", StringComparison.CurrentCultureIgnoreCase))
                    {
                        response = await client.PutAsync(url, new StringContent(body, Encoding.UTF8, "application/json"));
                    }
                    else if (_scrapper.Request.Method.Equals("post", StringComparison.CurrentCultureIgnoreCase))
                    {
                        response = await client.PostAsync(url, new StringContent(body, Encoding.UTF8, "application/json"));
                    }
                    else if(_scrapper.Request.Method.Equals("get", StringComparison.CurrentCultureIgnoreCase) || string.IsNullOrEmpty((_scrapper.Request.Method)))
                    {
                        response = await client.PostAsync(url, new StringContent(body, Encoding.UTF8, "application/json"));
                    }
                    else
                    {
                        return;
                    }
                        
                    var responseContent = await response.Content.ReadAsStringAsync();
                    await _scrapper.Process(new AjaxResponse(responseContent, url, body ));
                    await Execute(url);
                }
                _sw.Stop();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                Console.WriteLine(ex.ToString());
            }
        }

    }
}
