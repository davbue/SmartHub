using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmartHub.Clients;
using SmartHub.Models;

namespace SmartHub
{
    public class Program
    {
        public static void Main(string[] args)
        {
            InitDB();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static void InitDB()
        {
            using (var context = new SmartHomeContext())
            {
                // Creates the database if not exists
                context.Database.EnsureCreated();

                // Saves changes
                context.SaveChanges();
            }
        }
    }
}
