using Influunt.MongoStorage.Abstractions;
using MongoDB.Driver;

namespace Influunt.MongoStorage
{
    internal class MongoDbContext: IMongoDbContext
    {
        private IMongoDatabase _db { get; set; }
        private MongoClient _mongoClient { get; set; }
        public MongoDbContext(IMongoStorageConfiguration configuration)
        {
            var url = MongoUrl.Create(configuration.ConnectionString);
            _mongoClient = new MongoClient(url);
            _db = _mongoClient?.GetDatabase(url.DatabaseName);
        }

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            return _db.GetCollection<T>(name);
        }
    }
}