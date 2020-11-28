using Influunt.Storage.Mongo.Abstractions;
using MongoDB.Driver;

namespace Influunt.Storage.Mongo
{
    public class MongoDbContext: IMongoDbContext
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