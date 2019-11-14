using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace Elders.Cronus.Api.Playground
{
    class Program
    {
        static void Main(string[] args)
        {
            var cronusApi = CronusApi.GetHost(builder =>
            {
                builder.HostUrl = new Uri("https://localhost:7477");
                builder.Certificate = CertificateLoader.LoadFromStoreCert("*.local.com", "My", StoreLocation.LocalMachine, false);
            });
            cronusApi.RunAsync(CancellationToken.None);
        }
    }
}
