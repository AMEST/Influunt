﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Influunt.Feed.Entity;
using Influunt.MongoStorage.Abstractions;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Influunt.MongoStorage
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
            //ex. 5dc1039a1521eaa36835e541

            _dbCollection.DeleteOneAsync(Builders<TEntity>.Filter.Eq("_id", id));

        }

        public virtual void Update(TEntity obj)
        {
            _dbCollection.ReplaceOneAsync(Builders<TEntity>.Filter.Eq("_id", obj.Id), obj);
        }

        public async Task<TEntity> Get(string id)
        {
            //ex. 5dc1039a1521eaa36835e541

            FilterDefinition<TEntity> filter = Builders<TEntity>.Filter.Eq("_id", id);

            _dbCollection = _mongoContext.GetCollection<TEntity>(typeof(TEntity).Name);

            return await _dbCollection.FindAsync(filter).Result.FirstOrDefaultAsync();

        }

        public async Task<IEnumerable<TEntity>> Get()
        {
            var all = await _dbCollection.FindAsync(Builders<TEntity>.Filter.Empty);
            return await all.ToListAsync();
        }
    }
}