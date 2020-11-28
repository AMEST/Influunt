using Influunt.Storage.Mongo.Abstractions;

namespace Influunt.Storage
{
    public class MongoStorageConfiguration : IMongoStorageConfiguration
    {
        public string ConnectionString { get; set; }
    }
}