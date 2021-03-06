﻿using HtmlAgilityPack;
using Ripply.Components;
using Ripply.Util;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Ripply
{
    public class HtmlEngine
    {
        private static readonly Logger Log = new LoggerConfiguration().CreateLogger();
       
        private HtmlScrapper _scrapper;
        private string _siteBaseUrl;
        private HashSet<string> _links = new HashSet<string>();
        private HashSet<string> _existingLinks = new HashSet<string>();
        private readonly ILinkSite _linkSite;
        private HtmlLinkHandler _htmlLinkHandler;
        private bool newOnly = false;
       
        List<Task> allTasks = new List<Task>();
        SemaphoreSlim _throttler;
        ConcurrentQueue<string> queue = new ConcurrentQueue<string>();

        private int totalcounter = 0;

        private Stopwatch _sw;
        private string _userAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/36.0.1985.125 Safari/537.36";

        public HtmlEngine(HtmlScrapper scrapper, int threads =10)
        {
            _scrapper = scrapper;
            _linkSite = new LinkSiteFileWriter();
            _throttler = new SemaphoreSlim(initialCount: threads);
            _htmlLinkHandler = new HtmlLinkHandler(_scrapper);
        }

        public async Task CrawlAsync()
        {
            _siteBaseUrl = _scrapper.GetBaseUrl();
            await Execute(_scrapper.StartingUrl);
        }

        public async Task UpdateAsync( string filename = "")
        {
            _siteBaseUrl = _scrapper.GetBaseUrl();
            if (filename == "")
            {
                filename = _scrapper.DefaultFileName();
            }
            string[] urlFile = File.ReadAllLines(filename);
            _existingLinks = new HashSet<string>(urlFile.Select(s => s.Split(",")).Select(v => v[0]).ToList());
            await Execute(_scrapper.StartingUrl);
        }

        public async Task ItemsUpdateAsync(string filename = "", bool newOnly = false)
        {
            _siteBaseUrl = _scrapper.GetBaseUrl();
            if (filename == "")
            {
                filename = _scrapper.DefaultFileName();
            }
            string[] urlFile = File.ReadAllLines(filename);
            var urls = urlFile.Select(s => s.Split(",")).Where(v => v[1].Equals("True")).Select(v => v[0]).ToArray();
            await ReadItems(urls);
        }


        private async Task Execute(string url)
        {
            try
            {
                _sw = Stopwatch.StartNew();
                string webcontent;
                string href = _htmlLinkHandler.CleanQueryStringUrl(url);
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("user-agent", _userAgent);
                    foreach (var item in _scrapper.Request.Headers)
                    {
                        client.DefaultRequestHeaders.Add(item.Key, item.Value);
                    }
                    HttpResponseMessage response = await client.GetAsync(EngineHelper.EnsureCompleteUrl(_siteBaseUrl, href));
                    webcontent = await response.Content.ReadAsStringAsync();
                    if (_htmlLinkHandler.IsItemPage(url) && !newOnly)
                    {
                        var document = new HtmlDocument();
                        document.LoadHtml(webcontent);
                        await _scrapper.Process(new HtmlResponse(document, href));
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
        }

        private async Task TraverseBreadth()
        {
            EngineHelper.CheckStackTrace();
            while (queue.Count > 0)
            {
                totalcounter++;
                Console.WriteLine($"TotalCounter={totalcounter}");
                Console.WriteLine($"QueueCount={queue.Count}");
                Console.WriteLine($"ElapsedMilliseconds={_sw.Elapsed}");
                queue.TryDequeue(out var href);
                Console.WriteLine(href);
                Log.Information(href);
                await _throttler.WaitAsync();
                allTasks.Add(
                    Task.Run(async () =>
                    {
                        try
                        {
                            string traverseWebContent;
                            using (var client = new HttpClient())
                            {
                                client.DefaultRequestHeaders.Add("user-agent", _userAgent);
                                HttpResponseMessage response = await client.GetAsync(EngineHelper.EnsureCompleteUrl(_siteBaseUrl, href));
                                traverseWebContent = await response.Content.ReadAsStringAsync();
                                var document = new HtmlDocument();
                                document.LoadHtml(traverseWebContent);
                                if (_htmlLinkHandler.IsItemPage(href))
                                {
                                    await _scrapper.Process(new HtmlResponse(document, href));
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
                            _throttler.Release();
                        }
                    }));
            }
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
                                HttpResponseMessage response = await client.GetAsync(EngineHelper.EnsureCompleteUrl(_siteBaseUrl, url));
                                var traverseWebContent = await response.Content.ReadAsStringAsync();
                                var document = new HtmlDocument();
                                document.LoadHtml(traverseWebContent);
                                if (_htmlLinkHandler.IsItemPage(url))
                                {
                                    await _scrapper.Process(new HtmlResponse(document, url));
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

        private void AddLink(string href)
        {
            if (_existingLinks.Contains(href))
                return;

            if (_htmlLinkHandler.IsItemPage(href))
                AddLink(href, true);
            else
                AddLink(href, false);
        }

        private void AddLink(string href, bool isItem)
        {
            _linkSite.Create(_scrapper.SiteName, href, isItem);
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
                    string href = _htmlLinkHandler.CleanQueryStringUrl(matchHrefAttribute.Groups[1].Value);
                    if (_htmlLinkHandler.IsValidLink(href))
                    {
                        if (_links.Add(href))
                        {
                            queue.Enqueue(href);
                        }
                    }
                }
            }
        }

    
    }
}
