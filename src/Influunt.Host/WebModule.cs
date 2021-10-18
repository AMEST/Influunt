using System;
using System.IO;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Skidbladnir.Modules;

namespace Influunt.Host
{
    public class WebModule : Module
    {
        public override void Configure(IServiceCollection services)
        {
            services
                .AddDataProtection()
                .SetApplicationName("Influunt");
            services.AddControllers();
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                })
                .AddCookie()
                .AddGoogle(options =>
                {
                    IConfigurationSection googleAuthNSection =
                        Configuration.AppConfiguration.GetSection("Authentication:Google");

                    options.ClientId = googleAuthNSection["ClientId"];
                    options.ClientSecret = googleAuthNSection["ClientSecret"];
                    options.CorrelationCookie.SameSite = SameSiteMode.Lax;
                });
            // In production, the Vue files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
            services
                .AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Version = "v1",
                        Title = "Influunt API",
                        Description = "Influunt (Rss agregator) Api"
                    });
                    c.CustomSchemaIds(type => type.FullName);
                    var filePath = Path.Combine(AppContext.BaseDirectory, "Influunt.Host.xml");
                    if (File.Exists(filePath))
                        c.IncludeXmlComments(filePath);
                });
        }
    }
}