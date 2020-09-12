using Influunt.MongoStorage.Abstractions;

namespace Influunt.MongoStorage
{
    public class MongoStorageConfiguration : IMongoStorageConfiguration
    {
        public string ConnectionString { get; set; }
    }
}