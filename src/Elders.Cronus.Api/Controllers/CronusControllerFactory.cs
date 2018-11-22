using Elders.Cronus.MessageProcessing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Elders.Cronus.Api.Controllers
{
    public class CronusControllerFactory : IControllerFactory
    {
        private DefaultControllerFactory defaultControllerFactory;

        public CronusControllerFactory(IControllerActivator controllerActivator, IEnumerable<IControllerPropertyActivator> propertyActivators)
        {
            defaultControllerFactory = new DefaultControllerFactory(controllerActivator, propertyActivators);
        }

        public object CreateController(ControllerContext context)
        {
            var cronusContext = context.HttpContext.RequestServices.GetRequiredService<CronusContext>();
            if (cronusContext.IsNotInitialized)
            {
                var tenantClaim = context.HttpContext.User.Claims.Where(claim => claim.Type.Equals("tenant", StringComparison.OrdinalIgnoreCase) || claim.Type.Equals("tenant_client", StringComparison.OrdinalIgnoreCase) || claim.Type.Equals("client_tenant", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (tenantClaim is null == false)
                    cronusContext.Initialize(tenantClaim.Value, context.HttpContext.RequestServices);
            }

            return defaultControllerFactory.CreateController(context);
        }

        public void ReleaseController(ControllerContext context, object controller) => defaultControllerFactory.ReleaseController(context, controller);
    }
}
