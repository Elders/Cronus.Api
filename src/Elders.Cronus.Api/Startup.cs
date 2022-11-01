using Elders.Cronus.Api.Hubs;
using Elders.Cronus.Api.Security;
using Elders.Cronus.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;

namespace Elders.Cronus.Api
{
    public class Startup
    {
        string JwtSectionName = "Cronus:Api:JwtTenantConfig";

        private readonly IConfiguration configuration;
        private readonly bool authenticationEnabled = false;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.authenticationEnabled = configuration.GetSection(JwtSectionName).Exists();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSecurity(configuration);
            services.AddCronus(configuration);
            services.AddCronusAspNetCore();
            services.AddCronusApi();
            services.AddMonitor();

            services.AddCors(options => options.AddPolicy("CorsPolicy",
                builder =>
                {
                    builder.AllowAnyHeader()
                           .AllowAnyMethod()
                           .SetIsOriginAllowed((host) => true)
                           .AllowCredentials();
                }));

            services.AddSignalR();
            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseCors("CorsPolicy");

            app.UseResponseCompression();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            if (authenticationEnabled)
            {
                app.UseAuthentication();
                app.UseHttpsRedirection();
            }

            app.UseCronusAspNetCore(httpContext =>
            {
                return (
                    httpContext.Request.Path.Value.Contains("/domain/", System.StringComparison.OrdinalIgnoreCase) ||
                    httpContext.Request.Path.Value.Contains("/hub/", System.StringComparison.OrdinalIgnoreCase)
                ) == false;
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<RebuildProjectionHub>("/hub/projections");
            });
        }
    }

    public class AddAuthorizeFiltersControllerConvention : IControllerModelConvention
    {
        private readonly string globalScope;

        public AddAuthorizeFiltersControllerConvention(string globalScope)
        {
            this.globalScope = globalScope;
        }

        public void Apply(ControllerModel controller)
        {
            controller.Filters.Add(new AuthorizeFilter());
        }
    }
}
