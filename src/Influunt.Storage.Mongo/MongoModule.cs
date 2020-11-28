using Influunt.Storage.Mongo.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization.Conventions;

namespace Influunt.Storage.Mongo
{
    public static class MongoModule
    {
        public static IServiceCollection AddMongoModule(this IServiceCollection services,
            IMongoStorageConfiguration configuration)
        {
            // Register conventions
            var pack = new ConventionPack
            {
                new IgnoreIfDefaultConvention(true),
                new IgnoreExtraElementsConvention(true),
            };

            ConventionRegistry.Register("Influunt Convention", pack, t => true);

            services.AddSingleton(configuration);
            services.AddSingleton<IMongoDbContext, MongoDbContext>();

            return services;
        }
    }
}