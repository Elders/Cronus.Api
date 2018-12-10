﻿using Elders.Cronus.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Elders.Cronus.Api
{
    public class Startup
    {
        private readonly IConfiguration configuration;
        private readonly CronusApiBuilder cronusApiBuilder;

        public Startup(IConfiguration configuration, CronusApiBuilder cronusApiBuilder)
        {
            this.configuration = configuration;
            this.cronusApiBuilder = cronusApiBuilder;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(o =>
            {
                o.Conventions.Add(new AddAuthorizeFiltersControllerConvention("global-scope"));
            });
            services.AddCronusAspNetCore();

            if (cronusApiBuilder.CronusServicesProvider is null)
                services.AddCronus(configuration);
            else
                services.AddCronus(cronusApiBuilder.CronusServicesProvider(services, configuration));

            services.AddCronus(configuration);

            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                string authority = configuration["idsrv_authority"];
                o.Authority = authority;
                o.Audience = authority + "/resources";
                o.RequireHttpsMetadata = false;
            });

            services.AddCronusApi();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();
            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
            app.UseMvc();
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
