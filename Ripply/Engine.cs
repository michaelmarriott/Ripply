using HtmlAgilityPack;
using Ripply.Components;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Ripply
{
    public class Engine
    {
        private static readonly Logger Log = new LoggerConfiguration().CreateLogger();
        private IScrapper _scrapper;
        private HashSet<string> _links = new HashSet<string>();
        private HashSet<string> _existingLinks = new HashSet<string>();
        private readonly ILinkSite _linkSite;
        private string _userAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/36.0.1985.125 Safari/537.36";
        private string _siteBaseUrl = "";
        private bool newOnly = false;
        private Stopwatch _sw;
        private int totalcounter = 0;

        List<Task> allTasks = new List<Task>();
        SemaphoreSlim throttler = new SemaphoreSlim(initialCount: 10);
        ConcurrentQueue<string> queue = new ConcurrentQueue<string>();
        public Engine()
        {
            _linkSite = new LinkSiteFileWriter();
        }

        public async Task Run(string[] args)
        {
            var type = typeof(IScrapper);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p));

            IScrapper scrapper = null;
            string filename = "";

            string method = "";
            Console.WriteLine($".....................................");
            Console.WriteLine($"{args[0]}");
            for (var i = 0; i < args.Length; i++)
            {
                if (i == 0)
                {
                    scrapper = (IScrapper)Activator.CreateInstance(types.FirstOrDefault(x => x.Name == args[0]));
                }
                else
                {
                    switch (args[i])
                    {
                        case "-T":
                            {
                                method = args[i + 1];
                                i++;
                                break;
                            }
                        case "-F":
                            {
                                filename = args[i + 1];
                                i++;
                                break;
                            }
                        case "-N":
                            {
                                newOnly = true;
                                break;
                            }
                        default:
                            {
                                Console.WriteLine($"Unknown parameter {args[i]}");
                                return;
                            }
                    }
                }
            }

            switch (method.ToLower())
            {
                case "":
                case "crawl":
                    {
                        await CrawlAsync(scrapper);
                        break;
                    }

                case "update":
                    {
                        await UpdateAsync(scrapper, filename, newOnly);
                        break;
                    }
                case "itemsupdate":
                    {
                        await ItemsUpdateAsync(scrapper, filename);
                        break;
                    }
                default:
                    {
                        Console.WriteLine($"Unknown runtype {method}");
                        break;
                    }
            }
        }

        private async Task CrawlAsync(IScrapper scrapper)
        {
            _scrapper = scrapper;
            _siteBaseUrl = GetBaseUrl();
            await Execute(_scrapper.StartingUrl);
        }

        private async Task UpdateAsync(IScrapper scrapper, string filename = "", bool newOnly = false)
        {
            _scrapper = scrapper;
            _scrapper.NewOnly = newOnly;
            _siteBaseUrl = GetBaseUrl();
            if (filename == "")
            {
                filename = DefaultFileName();
            }
            string[] urlFile = File.ReadAllLines(filename);
            _existingLinks = new HashSet<string>(urlFile.Select(s => s.Split(",")).Select(v => v[0]).ToList());
            await Execute(_scrapper.StartingUrl);
        }

        private async Task ItemsUpdateAsync(IScrapper scrapper, string filename = "", bool newOnly = false)
        {
            _scrapper = scrapper;
            _siteBaseUrl = GetBaseUrl();
            if (filename == "")
            {
                filename = DefaultFileName();
            }
            string[] urlFile = File.ReadAllLines(filename);
            var urls = urlFile.Select(s => s.Split(",")).Where(v => v[1].Equals("True")).Select(v => v[0]).ToArray();
            await ReadItems(urls);
        }

        private string DefaultFileName()
        {
            return $"{_scrapper.SiteName}.csv";
        }

        private async Task<HashSet<string>> Execute(string url)
        {
            try

            {
                _sw = Stopwatch.StartNew();
                string webcontent;
                string href = CleanQueryStringUrl(url);
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("user-agent", _userAgent);
                    HttpResponseMessage response = await client.GetAsync(EnsureCompleteUrl(_siteBaseUrl, href));
                    webcontent = await response.Content.ReadAsStringAsync();
                    if (IsItemPage(url) && !newOnly)
                    {
                        var document = new HtmlDocument();
                        document.LoadHtml(webcontent);
                        await _scrapper.Process(new Response(document, href));
                    }
                }
                AddUrlsToQueue(webcontent);
                await TraverseBreadth();
                _sw.Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return _links;
        }

        private async Task TraverseBreadth()
        {
            CheckStackTrace();
            while (queue.Count > 0)
            {
                totalcounter++;
                Console.WriteLine($"TotalCounter={totalcounter}");
                Console.WriteLine($"QueueCount={queue.Count}");
                Console.WriteLine($"ElapsedMilliseconds={_sw.Elapsed}");
                queue.TryDequeue(out var href);
                Console.WriteLine(href);
                Log.Information(href);
                await throttler.WaitAsync();
                allTasks.Add(
                    Task.Run(async () =>
                    {
                        try
                        {
                            string traverseWebContent;
                            using (var client = new HttpClient())
                            {
                                client.DefaultRequestHeaders.Add("user-agent", _userAgent);
                                HttpResponseMessage response = await client.GetAsync(EnsureCompleteUrl(_siteBaseUrl, href));
                                traverseWebContent = await response.Content.ReadAsStringAsync();
                                var document = new HtmlDocument();
                                document.LoadHtml(traverseWebContent);
                                if (IsItemPage(href))
                                {
                                    await _scrapper.Process(new Response(document, href));
                                }
                            }
                            AddLink(href);
                            AddUrlsToQueue(traverseWebContent);
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex.ToString());
                        }
                        finally
                        {
                            throttler.Release();
                        }
                    }));
            }
        }

        static void CheckStackTrace()
        {
            StackTrace s = new StackTrace();
            Console.WriteLine($"StackTrace {s.FrameCount}");
        }

        private void AddUrlsToQueue(string webcontent)
        {
            MatchCollection matchs = Regex.Matches(webcontent, @"(<a.*?>.*?</a>)", RegexOptions.Singleline);
            foreach (var m in matchs)
            {
                string value = ((Match)m).Groups[1].Value;
                Match matchHrefAttribute = Regex.Match(value, @"href=\""(.*?)\""", RegexOptions.Singleline);
                if (matchHrefAttribute.Success)
                {
                    string href = CleanQueryStringUrl(matchHrefAttribute.Groups[1].Value);
                    if (IsValidLink(href))
                    {
                        if (_links.Add(href))
                        {
                            queue.Enqueue(href);
                        }
                    }
                }
            }
        }

        private void AddLink(string href)
        {
            if (_existingLinks.Contains(href))
                return;

            if (IsItemPage(href))
                AddLink(href, true);
            else
                AddLink(href, false);
        }

        private async Task ReadItems(string[] urls)
        {
            var allTasks = new List<Task>();
            var throttler = new SemaphoreSlim(initialCount: 20);
            foreach (var url in urls)
            {
                await throttler.WaitAsync();
                allTasks.Add(
                    Task.Run(async () =>
                    {
                        try
                        {
                            using (var client = new HttpClient())
                            {
                                client.DefaultRequestHeaders.Add("user-agent", _userAgent);
                                HttpResponseMessage response = await client.GetAsync(EnsureCompleteUrl(_siteBaseUrl, url));
                                var traverseWebContent = await response.Content.ReadAsStringAsync();
                                var document = new HtmlDocument();
                                document.LoadHtml(traverseWebContent);
                                if (IsItemPage(url))
                                {
                                    await _scrapper.Process(new Response(document, url));
                                }
                            }
                        }
                        finally
                        {
                            throttler.Release();
                        }
                    }));
            }
            await Task.WhenAll(allTasks);
        }

        private string CleanQueryStringUrl(string url)
        {
            if (!url.Contains("?"))
            {
                return RemoveIgnore(url);
            }

            var path = url.Split("?")[0];
            var querysting = url.Split("?")[1];
            if (_scrapper.QueryStringIncludeOnly != null)
            {
                var tempQueryString = querysting;
                foreach (var item in _scrapper.QueryStringIncludeOnly)
                {
                    querysting = Regex.Match(tempQueryString, item, RegexOptions.Singleline).ToString();
                    if (!String.IsNullOrEmpty(querysting))
                        break;
                }
            }
            else
            {
                path = RemoveIgnore(url);
                querysting = RemoveIgnore(querysting);
            }
            return $"{path}?{querysting}";
        }

        private string RemoveIgnore(string querysting)
        {
            if (_scrapper.QueryStringIgnore == null)
                return querysting;

            var tempQueryString = querysting;

            foreach (var item in _scrapper.QueryStringIgnore)
            {
                var ignore = Regex.Match(tempQueryString, item, RegexOptions.Singleline).ToString();
                if (!String.IsNullOrEmpty(ignore))
                    querysting = querysting.Replace(ignore, "");
            }
            return querysting;
        }

        private string GetBaseUrl()
        {
            return Regex.Match(_scrapper.StartingUrl, @"http[s]?:\/\/?([\da-z\.-]+)/", RegexOptions.Singleline).ToString();
        }

        private bool IsValidLink(string link)
        {
            if (_scrapper.InValidLinks != null)
            {
                foreach (var invalidLink in _scrapper.InValidLinks)
                {
                    if (link.Contains(invalidLink))
                    {
                        return false;
                    }
                }
            }

            foreach (var validLink in _scrapper.ValidLinks)
            {
                if (link.Contains(validLink))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsItemPage(string link)
        {
            foreach (var itemPage in _scrapper.ItemPage)
            {
                if (link.Contains(itemPage))
                {
                    return true;
                }
            }
            return false;
        }

        private void AddLink(string href, bool isItem)
        {
            _linkSite.Create(_scrapper.SiteName, href, isItem);
        }

        private static string EnsureCompleteUrl(string site, string link)
        {
            if (link.StartsWith("http://") || link.StartsWith("https://"))
            {
                return link;
            }
            return site + link;
        }

    }
}
