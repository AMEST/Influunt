using System.Collections.Generic;
using System.Threading.Tasks;

namespace Influunt.MongoStorage.Abstractions
{
    public interface IMongoRepository<TEntity> where TEntity : class
    {
        Task Create(TEntity obj);
        void Update(TEntity obj);
        void Delete(string id);
        Task<TEntity> Get(string id);
        Task<IEnumerable<TEntity>> Get();
    }
}