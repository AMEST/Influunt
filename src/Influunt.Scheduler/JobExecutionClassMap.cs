using Influunt.Scheduler.Abstractions.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

namespace Influunt.Scheduler;

public class JobExecutionClassMap : BsonClassMap<JobExecution>
{
    public JobExecutionClassMap()
    {
        AutoMap();
        MapIdMember(x => x.Id)
            .SetIdGenerator(StringObjectIdGenerator.Instance)
            .SetSerializer(new StringSerializer(BsonType.ObjectId));
        SetIgnoreExtraElements(true);
        MapProperty(x => x.Id)
            .SetIgnoreIfNull(true);
    }
}
