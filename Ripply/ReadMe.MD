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

{NameOfScrapper} -T <{TypeOfScrapper} -F {OverridedefaultlocationofFile}


-T {TypeOfScrapper}
---
"Crawler" (Default), this would normally be run first to create a state file
"Update", "this goes and seeks more valid pages and finds more products, from existing state"
"ProductUpdate", this ues the statefile to update Products in list




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
    public class MyScrapper : IScrapper
    {
        public MyScrapper()
        {
            this.SiteName = "MyScrapper";
            this.StartingUrl = "https://xxxxxxx/";
            this.ValidLinks = new[] { "/xxxxxx/", "/product-view/" };
            this.ItemPage = new[] { "/product-view/" };
            this.QueryStringIncludeOnly = new[] { "page=([0-9]+)" };
        }

        public override async Task Process(Response response)
        {
           //Do want you want to Response object
        }
	}
```
