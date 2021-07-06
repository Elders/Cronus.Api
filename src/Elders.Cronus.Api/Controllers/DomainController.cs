using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Elders.Cronus.Discoveries;
using Microsoft.AspNetCore.Authorization;
using System.Reflection;
using System.Runtime.Serialization;
using Elders.Cronus.Projections;

namespace Elders.Cronus.Api.Controllers
{
    [Route("domain")]
    [AllowAnonymous]
    public partial class DomainController : ApiControllerBase
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

        private bool HandlerRetrieveRequirements<T>(Type handlerCandidate)
        {
            return handlerCandidate.IsAbstract == false && handlerCandidate.IsInterface == false && typeof(T).IsAssignableFrom(handlerCandidate);
        }

        private ICollection<Gateway_Response> GetGateways(IEnumerable<Assembly> loadedAssemblies)
        {
            return RetrieveTypesFromAssemblies(loadedAssemblies,
                x => HandlerRetrieveRequirements<IGateway>(x),
                meta => new Gateway_Response
                {
                    Name = meta.Name,
                    Events = meta
                                .GetInterfaces()
                                    .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEventHandler<>))
                                    .SelectMany(ff => ff.GetGenericArguments()).Where(x => typeof(IEvent).IsAssignableFrom(x))
                                    .Select(x =>
                                        new Event_Response
                                        {
                                            Id = x.GetCustomAttribute<DataContractAttribute>().Name,
                                            Name = x.Name
                                        }).ToList()
                });
        }

        private ICollection<Port_Response> GetPorts(IEnumerable<Assembly> loadedAssemblies)
        {
            return RetrieveTypesFromAssemblies(loadedAssemblies,
                x => HandlerRetrieveRequirements<IPort>(x),
                meta => new Port_Response
                {
                    Id = meta.GetContractId(),
                    Name = meta.Name,
                    Events = meta
                                .GetInterfaces()
                                    .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEventHandler<>))
                                    .SelectMany(ff => ff.GetGenericArguments()).Where(x => typeof(IEvent).IsAssignableFrom(x))
                                    .Select(x =>
                                        new Event_Response
                                        {
                                            Id = x.GetCustomAttribute<DataContractAttribute>().Name,
                                            Name = x.Name
                                        }).ToList()
                });
        }

        private ICollection<Saga_Response> GetSagas(IEnumerable<Assembly> loadedAssemblies)
        {
            return RetrieveTypesFromAssemblies(loadedAssemblies,
               x => HandlerRetrieveRequirements<ISaga>(x) && x.GetCustomAttributes(typeof(DataContractAttribute), false).Length > 0,
               meta => new Saga_Response
               {
                   Id = meta.GetContractId(),
                   Name = meta.Name,
                   Events = meta
                                .GetInterfaces()
                                    .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEventHandler<>))
                                    .SelectMany(ff => ff.GetGenericArguments()).Where(x => typeof(IEvent).IsAssignableFrom(x))
                                    .Select(x =>
                                        new Event_Response
                                        {
                                            Id = x.GetCustomAttribute<DataContractAttribute>().Name,
                                            Name = x.Name
                                        }).ToList()
               });
        }

        private ICollection<Projection_Response> GetProjections(IEnumerable<Assembly> loadedAssemblies)
        {
            return RetrieveTypesFromAssemblies(loadedAssemblies,
               x => HandlerRetrieveRequirements<IProjection>(x) && x.GetCustomAttributes(typeof(DataContractAttribute), false).Length > 0,
               meta => new Projection_Response
               {
                   Id = meta.GetCustomAttribute<DataContractAttribute>().Name,
                   Name = meta.Name,
                   IsEventSourced = typeof(IAmEventSourcedProjection).IsAssignableFrom(meta),
                   Events = meta
                                .GetInterfaces()
                                    .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEventHandler<>))
                                    .SelectMany(ff => ff.GetGenericArguments()).Where(x => typeof(IEvent).IsAssignableFrom(x))
                                    .Select(x =>
                                        new Event_Response
                                        {
                                            Id = x.GetCustomAttribute<DataContractAttribute>().Name,
                                            Name = x.Name
                                        }).ToList()
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
                   Name = meta.Name,
                   Properties = meta.GetProperties(BindingFlags.Public).Select(x => x.Name).ToList()
               });
        }

        private ICollection<Aggregate_Response> GetAggregates(IEnumerable<System.Reflection.Assembly> loadedAssemblies)
        {
            return RetrieveTypesFromAssemblies(loadedAssemblies,
                x => HandlerRetrieveRequirements<IAggregateRoot>(x) && x != typeof(AggregateRoot<>),
                meta => new Aggregate_Response
                {
                    Name = meta.Name,
                    Commands = GetAggregateAppService(loadedAssemblies, meta)
                                    ?.GetInterfaces()
                                    ?.Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICommandHandler<>))
                                    ?.SelectMany(ff => ff.GetGenericArguments()).Where(x => typeof(ICommand).IsAssignableFrom(x))
                                    ?.Select(x =>
                                        new Command_Response
                                        {
                                            Id = x.GetCustomAttribute<DataContractAttribute>().Name,
                                            Name = x.Name
                                        })
                                    ?.ToList(),
                    Events = GetWhenEvents(GetAggregateState(meta))
                });
        }

        private ICollection<Event_Response> GetWhenEvents(Type type)
        {
            var allMethods = type.GetMethods()
                 .Where(x => x.GetParameters().Count() == 1 && typeof(IEvent).IsAssignableFrom(x.GetParameters().FirstOrDefault().ParameterType));

            return allMethods.Select(x =>
                                        new Event_Response
                                        {
                                            Id = x.GetParameters().FirstOrDefault().ParameterType.GetCustomAttribute<DataContractAttribute>().Name,
                                            Name = x.GetParameters().FirstOrDefault().ParameterType.Name
                                        }).ToList();
        }

        private Type GetAggregateState(Type meta)
        {
            return meta.BaseType.GetGenericArguments().FirstOrDefault(x => typeof(IAggregateRootState).IsAssignableFrom(x));
        }

        private Type GetAggregateAppService(IEnumerable<Assembly> loadedAssemblies, Type aggregate)
        {
            var d1 = typeof(ApplicationService<>);
            Type[] typeArgs = { aggregate };
            var makeme = d1.MakeGenericType(typeArgs);

            var allTypes = loadedAssemblies
                .SelectMany(ass => ass.GetLoadableTypes());

            var appService = allTypes
                .Where(x => makeme.IsAssignableFrom(x) && x.IsInterface == false && x.IsAbstract == false)
                .SingleOrDefault(); // Just imagine that there is an Aggregate without an AppService, see?

            return appService;
        }

    }
}
