using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Elders.Cronus.Api
{
    public static class CronusApi
    {
        public static IWebHost GetHost(IConfigurationSource additionalConfiguration = null, string hostUrl = null)
        {
            string url = hostUrl ?? "http://localhost:" + GetAvailablePort(9000) + "/";

            var webHostBuilder = WebHost
                .CreateDefaultBuilder()
                .UseKestrel()
                .UseUrls(url)
                .UseStartup<Startup>();

            if (additionalConfiguration is null == false)
                webHostBuilder.ConfigureAppConfiguration(cfg => cfg.Add(additionalConfiguration));

            var webHost = webHostBuilder.Build();

            return webHost;
        }

        public static void RunAsService(IConfigurationSource additionalConfiguration = null, string hostUrl = null)
        {
            GetHost(additionalConfiguration, hostUrl).RunAsService();
        }

        public static IWebHost Run(IConfigurationSource additionalConfiguration = null, string hostUrl = null)
        {
            var host = GetHost(additionalConfiguration, hostUrl);
            host.RunAsync();

            return host;
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
