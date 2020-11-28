using Influunt.Feed.Entity;
using Influunt.Storage.Mongo.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;

namespace Influunt.Storage.Mongo
{
    public static class RepositoryExtensions
    {
        public static IServiceCollection AddMongoRepository<TEntity, TEntityConfig>(
            this IServiceCollection services
        )
            where TEntity : class, IHasId
            where TEntityConfig : EntityMapClass<TEntity>, new()
        {
            services.AddSingleton<IMongoRepository<TEntity>, MongoRepository<TEntity>>();
            BsonClassMap.RegisterClassMap(new TEntityConfig());
            return services;
        }
    }
}