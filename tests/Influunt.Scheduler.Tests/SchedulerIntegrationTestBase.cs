using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Testcontainers.MongoDb;

namespace Influunt.Scheduler.Tests;

public abstract class SchedulerIntegrationTestBase : IAsyncLifetime
{
    private readonly MongoDbContainer _mongoContainer;
    private IHost _host;
    private IServiceProvider _serviceProvider;

    protected SchedulerIntegrationTestBase()
    {
        _mongoContainer = new MongoDbBuilder()
            .WithImage("mongo:latest")
            .Build();
    }

    protected IServiceProvider ServiceProvider => _serviceProvider ?? throw new InvalidOperationException("Test not initialized");

    protected T GetRequiredService<T>() where T : notnull
    {
        return ServiceProvider.GetRequiredService<T>();
    }

    public async Task InitializeAsync()
    {
        await _mongoContainer.StartAsync();
        
        var connectionStringUriBuilder = new UriBuilder(_mongoContainer.GetConnectionString());
        connectionStringUriBuilder.Path = "test-db";
        connectionStringUriBuilder.Query += "&authMechanism=SCRAM-SHA-256&authSource=admin";
        
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                ConfigureServices(services, connectionStringUriBuilder.ToString());
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            })
            .Build();

        _serviceProvider = _host.Services;

        await InitializeTestDataAsync();

        _host.StartAsync(CancellationToken.None);

        await Task.Delay(TimeSpan.FromMinutes(1)); // Time for index creation
    }

    protected virtual void ConfigureServices(IServiceCollection services, string connectionString)
    {
        // Override in derived tests to add additional services
    }

    protected virtual Task InitializeTestDataAsync()
    {
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        if (_host != null)
        {
            await _host.StopAsync();
            _host.Dispose();
        }

        await _mongoContainer.DisposeAsync();
    }
}