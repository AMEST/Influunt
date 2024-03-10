using Influunt.Feed.Entity;
using MongoDB.Bson;
using Skidbladnir.Repository.MongoDB;
using System;

namespace Influunt.Storage.Entity;

public class UserMap : EntityMapClass<User>
{
    public UserMap()
    {
        ToCollection("User");
        MapId(x => x.Id, BsonType.String);
        MapField(f => f.LastActivity)
            .SetDefaultValue(DateTime.UnixEpoch)
            .SetIgnoreIfDefault(false);
    }
}