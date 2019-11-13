using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Elders.Cronus.Discoveries;
using Microsoft.AspNetCore.Authorization;
using System.Reflection;
using System.Runtime.Serialization;
using Elders.Cronus.Projections;
using Elders.Cronus.Api.Playground.Domain.Samples.AppServices;
using Elders.Cronus.Api.Playground.Domain.Samples;

namespace Elders.Cronus.Api.Controllers
{
    [Route("domain")]
    [AllowAnonymous]
    public class DomainController : ApiControllerBase
    {
        [HttpGet, Route("explore")]
        public IActionResult Explore()
        {
            var result = new Domain_Response();
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(assembly => assembly.IsDynamic == false);

            result.Aggregates = GetAggregates(loadedAssemblies);
            result.Events = GetEvents(loadedAssemblies);
            result.Commands = GetCommands(loadedAssemblies);
            result.Projections = GetProjections(loadedAssemblies);
            result.Sagas = GetSagas(loadedAssemblies);
            result.Ports = GetPorts(loadedAssemblies);
            result.Gateways = GetGateways(loadedAssemblies);

            return new OkObjectResult(result);
        }

        private ICollection<Gateway_Response> GetGateways(IEnumerable<Assembly> loadedAssemblies)
        {
            return RetrieveTypesFromAssemblies(loadedAssemblies,
                x => typeof(IGateway).IsAssignableFrom(x) && x.IsInterface == false,
                meta => new Gateway_Response
                {
                    Name = meta.Name
                });
        }

        private ICollection<Port_Response> GetPorts(IEnumerable<Assembly> loadedAssemblies)
        {
            return RetrieveTypesFromAssemblies(loadedAssemblies,
                x => typeof(IPort).IsAssignableFrom(x) && x.IsInterface == false,
                meta => new Port_Response
                {
                    Name = meta.Name
                });
        }

        private ICollection<Saga_Response> GetSagas(IEnumerable<Assembly> loadedAssemblies)
        {
            return RetrieveTypesFromAssemblies(loadedAssemblies,
               x => typeof(Saga).IsAssignableFrom(x),
               meta => new Saga_Response
               {
                   Name = meta.Name,
               });
        }

        private ICollection<Projection_Response> GetProjections(IEnumerable<Assembly> loadedAssemblies)
        {
            return RetrieveTypesFromAssemblies(loadedAssemblies,
               x => typeof(IProjection).IsAssignableFrom(x) && x.GetCustomAttributes(typeof(DataContractAttribute), false).Length > 0,
               meta => new Projection_Response
               {
                   Id = meta.GetCustomAttribute<DataContractAttribute>().Name,
                   Name = meta.Name,
                   IsEventSourced = typeof(IAmEventSourcedProjection).IsAssignableFrom(meta)
               });
        }

        private ICollection<Command_Response> GetCommands(IEnumerable<Assembly> loadedAssemblies)
        {
            return RetrieveTypesFromAssemblies(loadedAssemblies,
               x => typeof(ICommand).IsAssignableFrom(x) && x.GetCustomAttributes(typeof(DataContractAttribute), false).Length > 0,
               meta => new Command_Response
               {
                   Id = meta.GetCustomAttribute<DataContractAttribute>().Name,
                   Name = meta.Name
               });
        }

        private ICollection<Event_Response> GetEvents(IEnumerable<Assembly> loadedAssemblies)
        {
            return RetrieveTypesFromAssemblies(loadedAssemblies,
               x => typeof(IEvent).IsAssignableFrom(x) && x.GetCustomAttributes(typeof(DataContractAttribute), false).Length > 0,
               meta => new Event_Response
               {
                   Id = meta.GetCustomAttribute<DataContractAttribute>().Name,
                   Name = meta.Name
               });
        }

        private ICollection<Aggregate_Response> GetAggregates(IEnumerable<System.Reflection.Assembly> loadedAssemblies)
        {
            return RetrieveTypesFromAssemblies(loadedAssemblies,
                x => typeof(IAggregateRoot).IsAssignableFrom(x) && x.IsInterface == false && x != typeof(AggregateRoot<>),
                meta => new Aggregate_Response
                {
                    Name = meta.Name,
                    Commands = GetAggregateAppService(loadedAssemblies, meta)
                                    .GetInterfaces()
                                    .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICommandHandler<>))
                                    .SelectMany(ff => ff.GetGenericArguments()).Where(x => typeof(ICommand).IsAssignableFrom(x))
                                    .Select(x => x.GetCustomAttribute<DataContractAttribute>().Name).ToList(),
                    Events = GetWhenEvents(GetAggregateState(loadedAssemblies, meta))

                }); ;
        }

        private ICollection<string> GetWhenEvents(Type type)
        {
            var allMethods = type.GetMethods()
                 .Where(x => x.GetParameters().Count() == 1 && typeof(IEvent).IsAssignableFrom(x.GetParameters().FirstOrDefault().ParameterType));

            return allMethods.Select(x => x.GetParameters().FirstOrDefault().ParameterType.GetCustomAttribute<DataContractAttribute>().Name).ToList();
        }

        private Type GetAggregateState(IEnumerable<Assembly> loadedAssemblies, Type meta)
        {
            return meta.BaseType.GetGenericArguments().FirstOrDefault(x => typeof(IAggregateRootState).IsAssignableFrom(x));
        }

        private Type GetAggregateAppService(IEnumerable<Assembly> loadedAssemblies, Type aggregate)
        {
            var d1 = typeof(ApplicationService<>);
            Type[] typeArgs = { aggregate };
            var makeme = d1.MakeGenericType(typeArgs);

            var appService = loadedAssemblies
                .SelectMany(ass => ass.GetLoadableTypes()
                .Where(x => makeme.IsAssignableFrom(x) && x.IsInterface == false && x.IsAbstract == false))
                .Single();

            return appService;
        }

        private ICollection<TResult> RetrieveTypesFromAssemblies<TResult>(IEnumerable<Assembly> loadedAssemblies, Func<Type, bool> typeFilter, Func<Type, TResult> retrieveResult)
        {
            var result = new List<TResult>();

            var typesMeta = loadedAssemblies
                .SelectMany(ass => ass.GetLoadableTypes()
                .Where(typeFilter));

            foreach (var typeMeta in typesMeta)
            {
                result.Add(retrieveResult(typeMeta));
            }

            return result;
        }

        public class Domain_Response
        {
            public Domain_Response()
            {
                Aggregates = new List<Aggregate_Response>();
            }

            public IEnumerable<Aggregate_Response> Aggregates { get; set; }

            public IEnumerable<Event_Response> Events { get; set; }

            public IEnumerable<Command_Response> Commands { get; set; }

            public IEnumerable<Projection_Response> Projections { get; set; }

            public IEnumerable<Saga_Response> Sagas { get; set; }

            public IEnumerable<Port_Response> Ports { get; set; }

            public IEnumerable<Gateway_Response> Gateways { get; set; }
        }

        public class Aggregate_Response : BaseDomainModel_Response
        {
            public ICollection<string> Events { get; set; }

            public IEnumerable<string> Commands { get; set; }
        }

        public class BaseDomainModel_Response
        {
            public string Name { get; set; }
        }

        public class BaseSerializableDomainModel_Response : BaseDomainModel_Response
        {
            /// <summary>
            /// Data contract name
            /// </summary>
            public string Id { get; set; }
        }

        public class Event_Response : BaseSerializableDomainModel_Response
        {
        }

        public class Command_Response : BaseSerializableDomainModel_Response
        {
        }

        public class Projection_Response : BaseSerializableDomainModel_Response
        {
            public bool IsEventSourced { get; set; }
        }

        public class Saga_Response : BaseDomainModel_Response
        {
        }

        public class Port_Response : BaseDomainModel_Response
        {

        }

        public class Gateway_Response : BaseDomainModel_Response
        {

        }
    }
}
