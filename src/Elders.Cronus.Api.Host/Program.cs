using System;

namespace Elders.Cronus.Api.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = CronusApi.GetHost();

            host.StartAsync();
            Console.WriteLine("Hello World!");
        }
    }
}
