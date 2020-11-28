namespace Influunt.Storage.Mongo.Abstractions
{
    public interface IMongoStorageConfiguration
    {
        string ConnectionString { get; set; }
    }
}