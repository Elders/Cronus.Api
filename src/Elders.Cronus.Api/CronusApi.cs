using System;
using System.Collections.Generic;
using System.Linq;
using Elders.Cronus.Api.Logging;
using Elders.Cronus.Discoveries;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Elders.Cronus.Api
{
    public class CronusApiBuilder
    {
        public Func<IServiceCollection, IConfiguration, CronusServicesProvider> CronusServicesProvider { get; set; }

        public IConfigurationSource AdditionalConfigurationSource { get; set; }

        public string HostUrl { get; set; }
    }

    public static class CronusApi
    {
        private static readonly ILog log = LogProvider.GetLogger(typeof(CronusApi));

        public static IWebHost GetHost(Action<CronusApiBuilder> builder = null)
        {
            var cronusApiBuilder = new CronusApiBuilder();
            if (builder != null)
                builder(cronusApiBuilder);


            int port = GetAvailablePort(9000);
            string url = cronusApiBuilder.HostUrl ?? "http://+:" + port + "/";

            log.Info(() => $"Starting Cronus API at {url}{Environment.NewLine}If you are not able to access it using DNS or public IP make sure that you have firewall rule and urlacl setup on the hosting machine.{Environment.NewLine} Example firewall: netsh advfirewall firewall add rule name=\"Cronus\" dir=in action=allow localport=9000-9010 protocol=tcp{Environment.NewLine} Example urlacl: netsh http add urlacl url={url} user=Everyone listen=yes");

            var webHostBuilder = WebHost.CreateDefaultBuilder();

            if (cronusApiBuilder.AdditionalConfigurationSource is null == false)
                webHostBuilder.ConfigureAppConfiguration(cfg => cfg.Add(cronusApiBuilder.AdditionalConfigurationSource));

            webHostBuilder.ConfigureServices(services => services.AddSingleton<CronusApiBuilder>(cronusApiBuilder));

            webHostBuilder
                .UseKestrel()
                .UseUrls(url)
                .UseStartup<Startup>();

            var webHost = webHostBuilder.Build();

            log.Info($"Cronus API url: {url}");

            return webHost;
        }

        private static int GetAvailablePort(int startingPort)
        {
            System.Net.IPEndPoint[] endPoints;
            List<int> portArray = new List<int>();

            var properties = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties();

            //getting active connections
            System.Net.NetworkInformation.TcpConnectionInformation[] connections = properties.GetActiveTcpConnections();
            portArray.AddRange(from n in connections
                               where n.LocalEndPoint.Port >= startingPort
                               select n.LocalEndPoint.Port);

            //getting active tcp listners - WCF service listening in tcp
            endPoints = properties.GetActiveTcpListeners();
            portArray.AddRange(from n in endPoints
                               where n.Port >= startingPort
                               select n.Port);

            //getting active udp listeners
            endPoints = properties.GetActiveUdpListeners();
            portArray.AddRange(from n in endPoints
                               where n.Port >= startingPort
                               select n.Port);

            portArray.Sort();

            for (int i = startingPort; i < UInt16.MaxValue; i++)
                if (!portArray.Contains(i))
                    return i;

            return 0;
        }
    }
}
