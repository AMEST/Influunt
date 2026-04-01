using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Influunt.Scheduler;

/// <summary>
/// MongoDB context implementation
/// </summary>
public class MongoContext : IMongoContext, IDisposable
{
    private readonly MongoUrl _url;
    private readonly Lazy<IMongoClient> _mongoClient;
    private IMongoDatabase _database;
    private bool _disposed;

    /// <summary>
    /// Creates a new instance of MongoContext with the specified connection string
    /// </summary>
    /// <param name="options">Job scheduler options</param>
    public MongoContext(IOptions<JobSchedulerOptions> options)
    {
        if (options?.Value?.ConnectionString == null)
            throw new ArgumentException("Connection string cannot be null or empty", nameof(options));

        var connectionString = options.Value.ConnectionString;
        
        // Connection string parsing
        _url = MongoUrl.Create(connectionString);
        
        // Lazy creation of MongoClient
        _mongoClient = new Lazy<IMongoClient>(() => new MongoClient(_url));
    }

    /// <summary>
    /// Gets the MongoDB database
    /// </summary>
    public IMongoDatabase Database => _database ??= _mongoClient.Value.GetDatabase(_url.DatabaseName);

    /// <summary>
    /// Gets the collection of the specified type
    /// </summary>
    /// <typeparam name="T">Type of collection documents</typeparam>
    /// <param name="collectionName">Collection name (if null, the type name will be used)</param>
    /// <returns>MongoDB collection</returns>
    public IMongoCollection<T> GetCollection<T>(string? collectionName = null)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(MongoContext));

        var name = collectionName ?? typeof(T).Name.ToLowerInvariant();
        return Database.GetCollection<T>(name);
    }

    /// <summary>
    /// Releases resources
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases resources
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Cleaning up managed resources
                if (_mongoClient.IsValueCreated)
                {
                    // MongoClient does not require explicit Dispose, but it can be added if needed
                }
            }

            _disposed = true;
        }
    }

    /// <summary>
    /// Finalizer
    /// </summary>
    ~MongoContext()
    {
        Dispose(false);
    }
}