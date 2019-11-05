using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Elders.Cronus.Api.Logging;
using Elders.Cronus.Discoveries;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Elders.Cronus.Api
{
    public class CronusApiBuilder
    {
        internal static Uri DefaulHttpUrl = new Uri("http://[::]:7477/");

        public Func<IServiceCollection, IConfiguration, CronusServicesProvider> CronusServicesProvider { get; set; }

        public IConfigurationSource AdditionalConfigurationSource { get; set; }

        public Uri HostUrl { get; set; }

        public X509Certificate2 Certificate { get; set; }
    }

    public static class CronusApi
    {
        private static readonly ILog log = LogProvider.GetLogger(typeof(CronusApi));

        public static IWebHost GetHost(Action<CronusApiBuilder> builder = null)
        {
            var cronusApiBuilder = new CronusApiBuilder();
            if (builder != null)
                builder(cronusApiBuilder);

            Uri url = cronusApiBuilder.HostUrl ?? CronusApiBuilder.DefaulHttpUrl;

            log.Info(() => $"Starting Cronus API at {url}{Environment.NewLine}If you are not able to access it using DNS or public IP make sure that you have firewall rule and urlacl setup on the hosting machine.{Environment.NewLine} Example firewall: netsh advfirewall firewall add rule name=\"Cronus\" dir=in action=allow localport=7477 protocol=tcp{Environment.NewLine} Example urlacl: netsh http add urlacl url={url} user=Everyone listen=yes");

            var webHostBuilder = WebHost.CreateDefaultBuilder();

            if (cronusApiBuilder.AdditionalConfigurationSource is null == false)
                webHostBuilder.ConfigureAppConfiguration(cfg => cfg.Add(cronusApiBuilder.AdditionalConfigurationSource));

            webHostBuilder.ConfigureServices(services => services.AddSingleton<CronusApiBuilder>(cronusApiBuilder));


            var webHost = webHostBuilder
                .ConfigureKestrel((context, options) =>
                {
                    options.Listen(IPAddress.Any, url.Port, listenOptions =>
                    {
                        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                        if (cronusApiBuilder.Certificate is null)
                        {
                            log.Info("Hosting Cronus API under http. It is recommended that you provide a certificate and a `HostUrl`.");
                        }
                        else
                        {
                            listenOptions.UseHttps(cronusApiBuilder.Certificate);
                        }
                    });
                })
                .UseStartup<Startup>()
                .Build();

            log.Info($"Cronus API url: {url}");

            return webHost;
        }
    }
}
