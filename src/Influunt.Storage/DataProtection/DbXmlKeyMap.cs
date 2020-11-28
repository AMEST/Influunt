using Influunt.Storage.Mongo;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

namespace Influunt.Storage.DataProtection
{
    internal class DbXmlKeyMap : EntityMapClass<DbXmlKey>
    {
        public DbXmlKeyMap()
        {
            ToCollection("dataProtectionKeys");

            MapProperty(x => x.Key).SetIsRequired(true);

            MapIdMember(x => x.Id)
                .SetIdGenerator(StringObjectIdGenerator.Instance)
                .SetSerializer(new StringSerializer(BsonType.ObjectId));


            MapCreator(x => new DbXmlKey(x.Id, x.KeyId, x.Key));
        }
    }

}