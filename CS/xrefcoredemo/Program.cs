using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using xrefcoredemo.Data;

namespace xrefcoredemo {
    public class Program {
        public static void Main(string[] args) {
            var host = CreateWebHostBuilder(args).Build();
            InitializeDb(host);
            host.Run();

        }

        static void InitializeDb(IWebHost host) {
            using(var scope = host.Services.CreateScope()) {
                var context = scope.ServiceProvider.GetRequiredService<SchoolContext>();
                DbInitializer.Initialize(context, new Reports.ReportsFactory());
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
        }
    }
}
