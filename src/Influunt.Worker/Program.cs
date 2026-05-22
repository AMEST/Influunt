using Influunt.Scheduler;
using Influunt.Storage;
using Influunt.Worker;
using Skidbladnir.Modules;

var builder = Host.CreateApplicationBuilder(args);
if (builder.Environment.IsDevelopment())
    builder.Configuration.AddUserSecrets<Program>();
builder.Services.AddOptions();
builder.Services.AddJobSchedulerHost(c =>
{
    builder.Configuration.GetSection("Scheduler").Bind(c);
    c.ConnectionString = builder.Configuration.GetSection("ConnectionStrings:Mongo").GetValue<string>("ConnectionString")!;
});
builder.Services.AddSkidbladnirModules<StartupModule>(configuration =>
{
    var storageConfiguration = builder.Configuration.GetSection("ConnectionStrings:Mongo").Get<StorageConfiguration>();
    configuration.Add(storageConfiguration);
}, builder.Configuration);

var host = builder.Build();
host.Run();
