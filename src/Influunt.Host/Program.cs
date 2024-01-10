using Influunt.Feed.Crawler;
using Influunt.Host;
using Influunt.Host.Configurations;
using Influunt.Storage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.SpaServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Skidbladnir.Modules;
using VueCliMiddleware;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOptions();
builder.Services.Configure<CrawlerOptions>(builder.Configuration.GetSection("FeedCrawler"));
var crawlerEnabled = builder.Configuration.GetSection("FeedCrawler:Enabled").Get<bool>();
if (crawlerEnabled)
    builder.Services.AddHostedService<FeedCrawlerBackgroundWorker>();

builder.Services.AddSkidbladnirModules<StartupModule>(configuration =>
{
    var storageConfiguration = builder.Configuration.GetSection("ConnectionStrings:Mongo").Get<StorageConfiguration>();
    configuration.Add(storageConfiguration);
    var redisConfiguration = builder.Configuration.GetSection("ConnectionStrings:Redis").Get<RedisConfiguration>();
    configuration.Add(redisConfiguration);
}, builder.Configuration);

var app = builder.Build();

var forwardedHeadersOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
    RequireHeaderSymmetry = false
};
forwardedHeadersOptions.KnownNetworks.Clear();
forwardedHeadersOptions.KnownProxies.Clear();
app.UseForwardedHeaders(forwardedHeadersOptions);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Influunt API");
    });
    app.UseDeveloperExceptionPage();
}

app.UseSpaStaticFiles();
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.MapToVueCliProxy(
        "{*path}",
        new SpaOptions { SourcePath = "ClientApp" },
        npmScript: "serve",
        regex: "Compiled successfully",
        forceKill: true
    );
}

app.UseSpa(spa =>
{
    spa.Options.SourcePath = "ClientApp";
});

app.Run();