using MongoDB.Driver;

namespace Influunt.Scheduler;

/// <summary>
/// Interface for working with MongoDB context
/// </summary>
public interface IMongoContext
{
    /// <summary>
    /// Gets the MongoDB database
    /// </summary>
    IMongoDatabase Database { get; }
    
    /// <summary>
    /// Gets the collection of the specified type
    /// </summary>
    /// <typeparam name="T">Type of collection documents</typeparam>
    /// <param name="collectionName">Collection name (if null, the type name will be used)</param>
    /// <returns>MongoDB collection</returns>
    IMongoCollection<T> GetCollection<T>(string collectionName = null);
    
    /// <summary>
    /// Releases resources
    /// </summary>
    void Dispose();
}