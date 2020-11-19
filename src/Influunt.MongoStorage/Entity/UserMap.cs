using System;
using Influunt.Feed.Entity;

namespace Influunt.MongoStorage.Entity
{
    public class UserMap: EntityMapClass<User>
    {
        public UserMap()
        {
            ToCollection("users");

            MapField(f => f.LastActivity)
                .SetDefaultValue(DateTime.UnixEpoch)
                .SetIgnoreIfDefault(false);
        }
    }
}