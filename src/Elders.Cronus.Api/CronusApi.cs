using System;
using System.Net;
using Elders.Cronus.Api.Logging;
using Elders.Cronus.Discoveries;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Elders.Cronus.Api
{
    public class CronusApiBuilder
    {
        public Func<IServiceCollection, IConfiguration, CronusServicesProvider> CronusServicesProvider { get; set; }

        public IConfigurationSource AdditionalConfigurationSource { get; set; }
    }

    public static class CronusApi
    {
        private static readonly ILog log = LogProvider.GetLogger(typeof(CronusApi));

        public static IHost GetHost(Action<CronusApiBuilder> builder = null)
        {
            var cronusApiBuilder = new CronusApiBuilder();
            if (builder != null)
                builder(cronusApiBuilder);

            log.Info(() => $"Starting Cronus API.{Environment.NewLine}If you are not able to access it using DNS or public IP make sure that you have firewall rule and urlacl setup on the hosting machine.{Environment.NewLine}Example firewall: netsh advfirewall firewall add rule name=\"Cronus\" dir=in action=allow localport=7477 protocol=tcp{Environment.NewLine}Example urlacl: netsh http add urlacl url=http://[::]:7477 user=Everyone listen=yes");

            var host = Host
                .CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddCronus(context.Configuration);
                    if (cronusApiBuilder.CronusServicesProvider is null == false)
                        services.AddCronus(cronusApiBuilder.CronusServicesProvider(services, context.Configuration));
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    if (cronusApiBuilder.AdditionalConfigurationSource is null == false)
                        webBuilder.ConfigureAppConfiguration(cfg => cfg.Add(cronusApiBuilder.AdditionalConfigurationSource));

                    webBuilder.UseKestrel((context, options) =>
                    {
                        IConfigurationSection kestrelSection = context.Configuration.GetSection("Cronus:Api:Kestrel");
                        if (kestrelSection.Exists())
                        {
                            options.Configure(kestrelSection);
                        }
                        else
                        {
                            options.Listen(IPAddress.Any, 7477, listenOptions =>
                            {
                                listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                            });
                        }
                    });

                    webBuilder.UseStartup<Startup>();
                })
                //.UseDefaultServiceProvider((context, options) =>
                //{
                //    options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
                //    options.ValidateOnBuild = false;
                //})

                .Build();

            return host;
        }
    }
}
