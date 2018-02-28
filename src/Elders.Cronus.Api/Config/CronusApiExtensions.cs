using System;
using Elders.Cronus.EventStore;
using Elders.Cronus.IocContainer;
using Elders.Cronus.Pipeline.Config;
using System.Collections.Generic;
using System.Linq;
using Elders.Cronus.Api.Converters;
using Elders.Cronus.Api.Logging;
using Elders.Cronus.Pipeline.Hosts;
using Elders.Cronus.Projections;

namespace Elders.Cronus.Api.Config
{
    public static class CronusApiExtensions
    {
        public static ICronusSettings UseCronusApi<T>(this T self, Action<ICronusApiSettings> configure = null) where T : ICronusSettings
        {
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            ICronusApiSettings settings = new EventStoreApiSettings(self);
            settings.Port = GetAvailablePort(9000);
            configure?.Invoke(settings);

            (settings as ISettingsBuilder).Build();
            return self;
        }

        // <summary>
        /// Checks for used ports and retrieves the first free port
        /// </summary>
        /// <returns>the free port or 0 if it did not find a free port</returns>
        public static int GetAvailablePort(int startingPort)
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

    public interface ICronusApiSettings : ICronusSettings
    {
        string BoundedContext { get; set; }

        int Port { get; set; }

        string EventStoreName { get; set; }

        string ProjectionName { get; set; }
    }

    public class EventStoreApiSettings : SettingsBuilder, ICronusApiSettings
    {
        static ILog log = LogProvider.GetLogger(typeof(EventStoreApiSettings));

        public EventStoreApiSettings(ISettingsBuilder settingsBuilder) : base(settingsBuilder) { }

        public override void Build()
        {
            var builder = this as ISettingsBuilder;
            ICronusApiSettings settings = this as ICronusApiSettings;

            settings.Container.RegisterSingleton(() => new EventStoreExplorer(settings.Container.Resolve<IEventStore>(builder.Name), settings.BoundedContext));

            var serviceLocator = new ServiceLocator(settings.Container);
            var controllerActivator = new CustomHttpControllerActivator(serviceLocator);

            Func<IProjectionRepository> projectionFactory = () => settings.Container.Resolve<IProjectionRepository>(settings.ProjectionName);
            Func<IEventStore> eventStoreFactory = () => settings.Container.Resolve<IEventStore>(settings.EventStoreName);

            settings.Container.RegisterSingleton<IProjectionRepository>(() => projectionFactory());
            settings.Container.RegisterSingleton<IEventStore>(() => eventStoreFactory());
            settings.Container.RegisterSingleton<ProjectionExplorer>(() => new ProjectionExplorer(projectionFactory()));

            try
            {
                log.Info(() => $"Starting Cronus API at http://+:{settings.Port}");
                var server = Microsoft.Owin.Hosting.WebApp.Start($"http://+:{settings.Port}", appBuilder => appBuilder.ConfigurEventStoreApi(controllerActivator));
                settings.Container.RegisterSingleton(() => server);
            }
            catch (System.Net.HttpListenerException ex)
            {
                string command = $"netsh http add urlacl url=http://+:{settings.Port}/ user=Everyone listen=yes";
                string exMessage = $"Unable to start Cronus API listener. You could Try: " + command;
                log.FatalException(exMessage, ex);
                throw new System.Reflection.TargetInvocationException(exMessage, ex);
            }
        }

        string ICronusApiSettings.BoundedContext { get; set; }

        int ICronusApiSettings.Port { get; set; }
        string ICronusApiSettings.EventStoreName { get; set; }
        string ICronusApiSettings.ProjectionName { get; set; }
    }
}
