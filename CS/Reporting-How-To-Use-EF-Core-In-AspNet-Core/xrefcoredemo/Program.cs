using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using xrefcoredemo.Data;

namespace xrefcoredemo
{
    public class Program
    {
        public static void Main(string[] args) {
            var host = CreateWebHostBuilder(args).Build();
            InitializeDb(host);
            host.Run();

        }

        static void InitializeDb(IWebHost host) {
            using(var scope = host.Services.CreateScope()) {
                var services = scope.ServiceProvider;
                try {
                    var context = services.GetRequiredService<SchoolContext>();
                    DbInitializer.Initialize(context, new Reports.ReportsFactory());
                } catch(Exception exception) {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(exception, "An error occurred while seeding the database.");
                }
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
        }
    }
}
