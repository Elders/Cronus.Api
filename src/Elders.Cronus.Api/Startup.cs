using Elders.Cronus.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Elders.Cronus.Api
{
    public class Startup
    {
        const string JwtSectionName = "Cronus:Api:JwtAuthentication";

        private readonly IConfiguration configuration;
        private readonly bool authenticationEnabled = false;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.authenticationEnabled = configuration.GetSection(JwtSectionName).Exists();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(o =>
            {
                if (authenticationEnabled)
                    o.Conventions.Add(new AddAuthorizeFiltersControllerConvention("global-scope"));
            });

            services.AddCronus(configuration);
            services.AddCronusAspNetCore();
            services.AddCronusApi();

            if (authenticationEnabled)
            {
                services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, configuration.GetSection(JwtSectionName));
                services.AddAuthentication(o =>
                {
                    o.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
                    o.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer();
            }
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            if (authenticationEnabled)
            {
                app.UseAuthentication();
                app.UseHttpsRedirection();
            }
            app.UseCronusAspNetCore();
            app.UseCors(policy => policy
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
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
