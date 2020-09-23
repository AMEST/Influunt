using Influunt.Feed.Entity;
using Influunt.MongoStorage.Abstractions;
using Influunt.MongoStorage.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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

        public static IServiceCollection AddMongoDataProtection(this IServiceCollection services)
        {
            return services
                .AddMongoRepository<DbXmlKey, DbXmlKeyMap>()
                .AddSingleton<IXmlRepository, MongoDbXmlRepository>()
                .AddSingleton<IConfigureOptions<KeyManagementOptions>, DataProtectionOptionsConfigurator>();
        }
    }
}