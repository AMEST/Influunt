using Influunt.MongoStorage.Abstractions;
using MongoDB.Driver;

namespace Influunt.MongoStorage
{
    internal class MongoDbContext: IMongoDbContext
    {
        private IMongoDatabase Db { get; set; }
        private MongoClient MongoClient { get; set; }

        public MongoDbContext(IMongoStorageConfiguration configuration)
        {
            var url = MongoUrl.Create(configuration.ConnectionString);
            MongoClient = new MongoClient(url);
            Db = MongoClient?.GetDatabase(url.DatabaseName);
        }

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            return Db.GetCollection<T>(name);
        }
    }
}