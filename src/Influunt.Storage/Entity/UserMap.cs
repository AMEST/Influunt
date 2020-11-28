using System;
using Influunt.Feed.Entity;
using Influunt.Storage.Mongo;

namespace Influunt.Storage.Entity
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