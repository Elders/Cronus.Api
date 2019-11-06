using Elders.Cronus.Multitenancy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace Elders.Cronus.Api.Host
{
    class Program
    {
        static void Main(string[] args)
        {

            var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            var gg = builder.Sources.First();

            var host = CronusApi.GetHost();

            host.StartAsync();
            Console.WriteLine("Hello World!");
        }
    }
}
