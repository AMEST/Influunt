using Influunt.Feed.Rss;
using Influunt.MongoStorage;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VueCliMiddleware;

namespace Influunt.Host
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                })
                .AddCookie()
                .AddGoogle(options =>
                {
                    IConfigurationSection googleAuthNSection =
                        Configuration.GetSection("Authentication:Google");

                    options.ClientId = googleAuthNSection["ClientId"];
                    options.ClientSecret = googleAuthNSection["ClientSecret"];
                    _ = options.Scope;
                });
            // In production, the Vue files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
            //AddModules

            var storageCfg = Configuration.GetSection("ConnectionStrings:Mongo").Get<MongoStorageConfiguration>();
            services.AddRssModule();
            services.AddMongoStorage(storageCfg);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSpaStaticFiles();

            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                if (env.IsDevelopment())
                {
                    endpoints.MapToVueCliProxy(
                        "{*path}",
                        new SpaOptions { SourcePath = "ClientApp"},
                        npmScript: "serve",
                        regex: "Compiled successfully",
                        forceKill: true
                    );
                }
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";
            });
        }
    }
}