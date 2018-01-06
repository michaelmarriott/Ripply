using Serilog;
using Serilog.Core;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Ripply
{
    public class EngineContainer
    {
        private static readonly Logger Log = new LoggerConfiguration().CreateLogger();

        public EngineContainer() {}

        public async Task Run(string[] args)
        {

            var arg = ExecuteArguments(args);

            if (arg.scrapper is HtmlScrapper)
            {
                var engine = new HtmlEngine((HtmlScrapper)arg.scrapper,arg.threads);
                switch (arg.method.ToLower())
                {
                    case "":
                    case "crawl":
                        {
                            await engine.CrawlAsync();
                            break;
                        }

                    case "update":
                        {
                            await engine.UpdateAsync(arg.filename);
                            break;
                        }
                    case "itemsupdate":
                        {
                            await engine.ItemsUpdateAsync(arg.filename,arg.newOnly);
                            break;
                        }
                    default:
                        {
                            Console.WriteLine($"Unknown runtype {arg.method} for Html Scrapper");
                            break;
                        }
                }
            }
            else
            {
                AjaxEngine engine = new AjaxEngine((AjaxScrapper)arg.scrapper);
                switch (arg.method.ToLower())
                {
                    case "":
                    case "crawl":
                        {
                            await engine.CrawlAsync();
                            break;
                        }
                    default:
                        {
                            Console.WriteLine($"Unknown runtype {arg.method} for Ajax Scrapper");
                            break;
                        }
                }
            }
           
        }


        private (DefaultScrapper scrapper, string filename, string method,bool newOnly, int threads) ExecuteArguments(string[] args)
        {
            DefaultScrapper scrapper = null;
            string filename = "";
            string method = "";
            var threads = 10;
            var newOnly = false;
            var type = typeof(DefaultScrapper);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p));

            Console.WriteLine($"{args[0]}");

            for (var i = 0; i < args.Length; i++)
            {
                if (i == 0)
                {
                    var typed = types.FirstOrDefault(x => x.Name == args[0]);
                    scrapper = (DefaultScrapper)Activator.CreateInstance(typed);
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
                        case "-TD":
                            {
                                threads = Int32.Parse(args[i + 1]);
                                i++;
                                break;
                            }
                        default:
                            {
                                Console.WriteLine($"Unknown parameter {args[i]}");
                                return (null, null, null,false, 0);
                            }
                    }
                }
            }
            return (scrapper, filename, method, newOnly, threads);
        }

    }
}
