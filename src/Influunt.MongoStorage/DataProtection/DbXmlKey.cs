using Influunt.Feed.Entity;

namespace Influunt.MongoStorage.DataProtection
{
    internal class DbXmlKey : IHasId
    {
        public DbXmlKey(string id, string keyId, string key)
        {
            Id = id;
            KeyId = keyId;
            Key = key;
        }

        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
        public string KeyId { get; private set; }

        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
        public string Key { get; private set; }

        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
        public string Id { get; private set; }

    }
}