﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Influunt.Storage.Mongo.Abstractions
{
    public interface IMongoRepository<TEntity> where TEntity : class
    {
        Task Create(TEntity obj);
        void Update(TEntity obj);
        void Delete(string id);
        Task<TEntity> Get(string id);
        IQueryable<TEntity> GetAll();
    }
}