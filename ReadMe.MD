Introduction
----

Ripply, is a .net crawler/scrapper
It creates a state file per Scrapper, this state file helps when you dont wish to crawl the site over and aver again.

Install
---
Install-Package Ripply



Download source and create from Template for new solution
---
dotnet new --install {$SourceFolderLocation}/Template

dotnet new ripply



Building,Publishing and Running
---

dotnet build

dotnet publish

dotnet run xxxxxxx -T update



Command line usage:
---

{NameOfScrapper} -T {TypeOfScrapper} -F {OverridedefaultlocationofFile} -TD {NumberOfThreads(int)}


-T {TypeOfScrapper}
---
"Crawler" (Default), this would normally be run first to create a state file
"Update", "this goes and seeks more valid pages and finds more items, from existing state"
"ItemsUpdate", this ues the statefile to update Items in list

 -TD {NumberOfThreads(int)}
 ---
 Default is 10 threads


Implmentation (rather just use template)
---

```C#
    class Program
    {
        public static async Task Main(string[] args)
        {
            var engine = new Engine();
            if (args.Length > 0)
            {
                await engine.Run(args);
            }
           
        }
    }
```

```C#
using System;
using System.Threading.Tasks;

namespace Ripply.Scrapper.Scrappers
{
    public class MyFirstScrapper : IScrapper
    {
        public MyFirstScrapper()
        {
            this.SiteName = "MyFirst";
            this.StartingUrl = "https://xxxxxxxxxxxx";
            this.ValidLinks = new [] { "" }; //This will only follow links that contain information specified in VlaidLinks
            this.ItemPage = new [] { "" }; //What does the url for the pages you wish find look like
            this.QueryStringIncludeOnly = new[] { "" }; //regex on querystring
        }
        
        public override async Task Process(Response response)
        {
			Console.WriteLine($"Url:{response.Url}");
            Console.WriteLine($"Document String:{response.ToString()}");
            //response.Css("div", "class", "id");
            //response.Meta("name");
        }
    }
}
```
