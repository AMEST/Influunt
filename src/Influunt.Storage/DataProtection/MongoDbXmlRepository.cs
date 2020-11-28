using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Influunt.Storage.Mongo.Abstractions;
using Microsoft.AspNetCore.DataProtection.Repositories;

namespace Influunt.Storage.DataProtection
{
    internal class MongoDbXmlRepository : IXmlRepository
    {
        private readonly IMongoRepository<DbXmlKey> _keys;

        public MongoDbXmlRepository(IMongoRepository<DbXmlKey> keys)
        {
            _keys = keys;
        }

        public IReadOnlyCollection<XElement> GetAllElements()
        {
            var keys = _keys.Get().GetAwaiter().GetResult();
            return keys.Select(key => key.ToKeyXmlElement()).ToList();
        }

        public void StoreElement(XElement element, string friendlyName)
        {
            _keys.Create(element.ToNewDbXmlKey()).GetAwaiter().GetResult();
        }
    }
}