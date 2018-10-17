//using System;
//using System.Linq;
//using System.Net.Http;
//using System.Reflection;
//using System.Web.Http;
//using System.Web.Http.Controllers;
//using System.Web.Http.Cors;
//using System.Web.Http.Dispatcher;
//using System.Web.Http.ModelBinding;
//using System.Web.Http.ValueProviders;
//using Elders.Cronus.Api.Converters;
//using Elders.Cronus.IocContainer;
//using Elders.Web.Api.Filters;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using Owin;

//namespace Elders.Cronus.Api
//{
//    public static class EventStorApiConfiguration
//    {
//        public static void ConfigurEventStoreApi(this IAppBuilder appBuilder, IHttpControllerActivator customControllerActivator)
//        {
//            // Configure Web API for self-host. 
//            HttpConfiguration config = new HttpConfiguration();
//            //config.MapHttpAttributeRoutes();
//            config.Formatters.Remove(config.Formatters.XmlFormatter);
//            config.Formatters.Remove(config.Formatters.FormUrlEncodedFormatter);
//            config.ConfigureJsonSerializer();
//            config.EnableCors(new EnableCorsAttribute("*", "*", "GET,POST"));
//            config.Routes.MapHttpRoute(
//                name: "DefaultApi",
//                routeTemplate: "api/{controller}/{id}",
//                defaults: new { id = RouteParameter.Optional }
//            );

//            if (ReferenceEquals(null, customControllerActivator) == false)
//                config.Services.Replace(typeof(System.Web.Http.Dispatcher.IHttpControllerActivator), customControllerActivator);

//            appBuilder.UseWebApi(config);
//        }

//        static HttpConfiguration ConfigureJsonSerializer(this HttpConfiguration config)
//        {
//            JsonSerializerSettings settings = config.Formatters.JsonFormatter.SerializerSettings;
//            //settings.Converters.Add(new ErrorConverter(() => HttpContext.Current.GetOwinContext()));
//            settings.NullValueHandling = NullValueHandling.Ignore;
//            settings.Formatting = Formatting.Indented;
//            settings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();

//            var converters = typeof(StringTenantIdConverter).Assembly.GetTypes()
//                .Where(x => typeof(JsonConverter).IsAssignableFrom(x) && x.IsAbstract == false);
//            foreach (var item in converters)
//            {
//                settings.Converters.Add(Activator.CreateInstance(item) as JsonConverter);
//            }

//            return config;
//        }
//    }

//    public class CustomHttpControllerActivator : System.Web.Http.Dispatcher.IHttpControllerActivator
//    {
//        readonly ServiceLocator serviceLocator;

//        public CustomHttpControllerActivator(ServiceLocator serviceLocator)
//        {
//            this.serviceLocator = serviceLocator;
//        }

//        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
//        {
//            var controller = serviceLocator.Resolve(controllerType) as IHttpController;

//            return controller;
//        }
//    }

//    public class ServiceLocator
//    {
//        IContainer container;

//        public ServiceLocator(IContainer container)
//        {
//            this.container = container;
//        }

//        public object Resolve(Type objectType)
//        {
//            var instance = FastActivator.CreateInstance(objectType);
//            var props = objectType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).ToList();
//            var dependencies = props.Where(x => container.IsRegistered(x.PropertyType));
//            foreach (var item in dependencies)
//            {
//                item.SetValue(instance, container.Resolve(item.PropertyType));
//            }
//            return instance;
//        }

//        public T Resolve<T>()
//        {
//            return (T)Resolve(typeof(T));
//        }
//    }

//    public class UrlBinder : IModelBinder
//    {
//        public UrlBinder()
//        {
//        }
//        static JsonSerializer serializer;
//        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
//        {
//            var converters = this.GetType().Assembly.GetTypes().Where(x => typeof(JsonConverter).IsAssignableFrom(x) && x.IsAbstract == false).Select(x => Activator.CreateInstance(x) as JsonConverter).ToList();
//            var instance = Activator.CreateInstance(bindingContext.ModelType);
//            var properties = bindingContext.ModelType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
//            var obj = JObject.FromObject(instance);

//            foreach (var item in properties)
//            {
//                ValueProviderResult val = bindingContext.ValueProvider.GetValue(item.Name.ToLowerInvariant());
//                if (val == null)
//                    continue;
//                //var converter = converters.First(x => x.CanConvert(item.PropertyType) && x.CanRead);
//                bool isOfType = val.RawValue is string || val.RawValue is int;
//                int intValue;
//                if (int.TryParse(val.RawValue.ToString(), out intValue))
//                {
//                    obj[item.Name] = JToken.FromObject(intValue);
//                }
//                else
//                    obj[item.Name] = JToken.FromObject(val.RawValue);

//            }
//            if (serializer == null)
//                serializer = JsonSerializer.CreateDefault(actionContext.RequestContext.Configuration.Formatters.JsonFormatter.SerializerSettings);
//            //actionContext.RequestContext.Configuration.Formatters.JsonFormatter.CreateJsonSerializer()
//            // var token = JToken.FromObject(obj);
//            var result = serializer.Deserialize(JToken.Parse(obj.ToString(Formatting.None)).CreateReader(), bindingContext.ModelType);
//            if (result == null)
//                return false;
//            bindingContext.Model = result;

//            return true;
//        }
//    }
//}
