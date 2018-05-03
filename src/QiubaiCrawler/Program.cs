using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace QiubaiCrawler
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // BuildWebHost(args).Run();

            string url = "https://www.qiushibaike.com/article/120354746";
            var services = new Services.QiubaiCrawlerService();
            var model = services.ProcessPageAsync(url).Result;
            Console.WriteLine(model.ToString());
            Console.ReadLine();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
