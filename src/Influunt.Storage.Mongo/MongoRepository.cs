using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Influunt.Feed.Entity;
using Influunt.Storage.Mongo.Abstractions;
using MongoDB.Driver;

namespace Influunt.Storage.Mongo
{
    internal class MongoRepository<TEntity> : IMongoRepository<TEntity> where TEntity : class, IHasId
    {
        private readonly IMongoDbContext _mongoContext;
        private IMongoCollection<TEntity> _dbCollection;

        public MongoRepository(IMongoDbContext context)
        {
            _mongoContext = context;
            _dbCollection = _mongoContext.GetCollection<TEntity>(typeof(TEntity).Name);
        }

        public async Task Create(TEntity obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(typeof(TEntity).Name + " object is null");
            }

            _dbCollection = _mongoContext.GetCollection<TEntity>(typeof(TEntity).Name);
            await _dbCollection.InsertOneAsync(obj);
        }

        public void Delete(string id)
        {
            _dbCollection.DeleteOneAsync(Builders<TEntity>.Filter.Eq("_id", id));
        }

        public virtual void Update(TEntity obj)
        {
            _dbCollection.ReplaceOneAsync(Builders<TEntity>.Filter.Eq("_id", obj.Id), obj);
        }

        public async Task<TEntity> Get(string id)
        {
            var filter = Builders<TEntity>.Filter.Eq("_id", id);

            _dbCollection = _mongoContext.GetCollection<TEntity>(typeof(TEntity).Name);

            return await _dbCollection.FindAsync(filter).Result.FirstOrDefaultAsync();
        }

        public IQueryable<TEntity> GetAll()
        {
            return _dbCollection.AsQueryable();
        }
    }
}