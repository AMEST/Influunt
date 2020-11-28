using MongoDB.Driver;

namespace Influunt.Storage.Mongo.Abstractions
{
    public interface IMongoDbContext
    {
#pragma warning disable 693
        IMongoCollection<T> GetCollection<T>(string name);
#pragma warning restore 693
    }
}