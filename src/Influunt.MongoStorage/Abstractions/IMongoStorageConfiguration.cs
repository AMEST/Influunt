namespace Influunt.MongoStorage.Abstractions
{
    public interface IMongoStorageConfiguration
    {
        string ConnectionString { get; set; }
    }
}