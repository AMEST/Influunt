using Influunt.Feed.Entity;
using Influunt.MongoStorage.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;

namespace Influunt.MongoStorage
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