using System.Threading.Tasks;

namespace Ripply.Scrapper
{
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
}
