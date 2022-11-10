using Elders.Cronus.AspNetCore.Exeptions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace Elders.Cronus.Api.Security
{
    public static class Security
    {
        private static readonly string JwtSectionName = "Cronus:Api:JwtTenantConfig";
        private static bool authenticationEnabled = false;

        public static IServiceCollection AddSecurity(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddBasicSecurity();
            services.AddMvcSecurity(configuration);
            services.AddJwtSecurity(configuration);

            return services;
        }

        private static IServiceCollection AddBasicSecurity(this IServiceCollection services)
        {
            services.AddAuthorization(o =>
            {
                o.AddPolicy(BasicAuthorizationAttribute.PolicyName, new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());
            });

            services
                .AddAuthentication()
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(BasicAuthorizationAttribute.AuthenticationSchema, options => { });

            return services;
        }

        private static IServiceCollection AddMvcSecurity(this IServiceCollection services, IConfiguration configuration)
        {
            authenticationEnabled = configuration.GetSection(JwtSectionName).Exists();

            services.AddMvc(o =>
            {
                if (authenticationEnabled)
                    o.Conventions.Add(new AddAuthorizeFiltersControllerConvention(Scope.Global));

                HttpNoContentOutputFormatter noContentFormatter = o.OutputFormatters.OfType<HttpNoContentOutputFormatter>().FirstOrDefault();
                if (noContentFormatter != null)
                {
                    noContentFormatter.TreatNullValueAsNoContent = false;
                }
            })
           .AddNewtonsoftJson();

            return services;
        }

        private static IServiceCollection AddJwtSecurity(this IServiceCollection services, IConfiguration configuration)
        {
            authenticationEnabled = configuration.GetSection(JwtSectionName).Exists();

            if (authenticationEnabled)
            {
                List<TenantJWTOptions> tenantConfigurations = new List<TenantJWTOptions>();
                configuration.GetRequiredSection(JwtSectionName).Bind(tenantConfigurations);

                services.AddAuthorization(opt =>
                {
                    foreach (TenantJWTOptions tenantConfig in tenantConfigurations)
                    {
                        opt.AddPolicy(Scope.Global, policy => policy.Requirements.Add(new HasScopeRequirement(Scope.Global, tenantConfig.JwtBearerOptions.Authority)));
                    }
                });

                AuthenticationBuilder builder = services.AddAuthentication(opt =>
                {
                    opt.DefaultScheme = "MultitenantJwtBearerSchema";
                    opt.DefaultChallengeScheme = "MultitenantJwtBearerSchema";
                });

                foreach (TenantJWTOptions tenantConfig in tenantConfigurations)
                {
                    builder = builder.AddTenantJwtBearer(tenantConfig.Name, tenantConfig.JwtBearerOptions);
                }

                builder.AddPolicyScheme("MultitenantJwtBearerSchema", "MultitenantJwtBearerSchema", opt =>
                {
                    opt.ForwardDefaultSelector = context =>
                    {
                        string authorization = context.Request.Headers[HeaderNames.Authorization];
                        if (string.IsNullOrEmpty(authorization) == false && authorization.StartsWith("Bearer ", System.StringComparison.OrdinalIgnoreCase))
                        {
                            string token = authorization.Substring("Bearer ".Length).Trim();
                            JwtSecurityTokenHandler jwtHandler = new JwtSecurityTokenHandler();

                            if (jwtHandler.CanReadToken(token))
                            {
                                if (jwtHandler.ReadJwtToken(token).Payload.TryGetValue("tenant", out object tenant))
                                    return tenant.ToString();
                            }
                        }

                        return BasicAuthorizationAttribute.AuthenticationSchema;
                    };
                });

                services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
            }

            return services;
        }

        private static AuthenticationBuilder AddTenantJwtBearer(this AuthenticationBuilder authenticationBuilder, string tenant, JwtBearerOptions jwtBearerOptions)
        {
            return authenticationBuilder.AddJwtBearer(tenant, o =>
            {
                o.Authority = jwtBearerOptions.Authority;
                o.Audience = jwtBearerOptions.Audience;
                o.RequireHttpsMetadata = jwtBearerOptions.RequireHttpsMetadata;
            });
        }
    }

    public class TenantJWTOptions
    {
        public string Name { get; set; }
        public JwtBearerOptions JwtBearerOptions { get; set; }
    }
}
