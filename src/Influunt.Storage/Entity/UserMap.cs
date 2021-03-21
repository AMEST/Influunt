using Influunt.Feed.Entity;
using Skidbladnir.Repository.MongoDB;
using System;

namespace Influunt.Storage.Entity
{
    public class UserMap : EntityMapClass<User>
    {
        public UserMap()
        {
            ToCollection("User");

            MapField(f => f.LastActivity)
                .SetDefaultValue(DateTime.UnixEpoch)
                .SetIgnoreIfDefault(false);
        }
    }
}