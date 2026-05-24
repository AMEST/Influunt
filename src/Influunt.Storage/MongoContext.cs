using System;
using MongoDB.Driver;

namespace Influunt.Storage;

/// <summary>
/// Реализация контекста MongoDB
/// </summary>
public class MongoContext : IMongoContext, IDisposable
{
    private readonly MongoUrl _url;
    private readonly Lazy<IMongoClient> _mongoClient;
    private IMongoDatabase _database;
    private bool _disposed;

    /// <summary>
    /// Создает новый экземпляр MongoContext с указанной строкой подключения
    /// </summary>
    /// <param name="connectionString">Строка подключения к MongoDB</param>
    public MongoContext(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Connection string cannot be null or empty", nameof(connectionString));

        // Парсинг строки подключения
        _url = MongoUrl.Create(connectionString);
        
        // Lazy создание MongoClient
        _mongoClient = new Lazy<IMongoClient>(() => new MongoClient(_url));
    }

    /// <summary>
    /// Получает базу данных MongoDB
    /// </summary>
    public IMongoDatabase Database => _database ??= _mongoClient.Value.GetDatabase(_url.DatabaseName);

    /// <summary>
    /// Получает коллекцию указанного типа
    /// </summary>
    /// <typeparam name="T">Тип документов коллекции</typeparam>
    /// <param name="collectionName">Имя коллекции (если null, будет использовано имя типа)</param>
    /// <returns>Коллекция MongoDB</returns>
    public IMongoCollection<T> GetCollection<T>(string collectionName = null)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(MongoContext));

        var name = collectionName ?? typeof(T).Name.ToLowerInvariant();
        return Database.GetCollection<T>(name);
    }

    /// <summary>
    /// Освобождает ресурсы
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Освобождает ресурсы
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Очищаем управляемые ресурсы
                if (_mongoClient.IsValueCreated)
                {
                    // MongoClient не требует явного Dispose, но если нужно, можно добавить
                }
            }

            _disposed = true;
        }
    }

    /// <summary>
    /// Финализатор
    /// </summary>
    ~MongoContext()
    {
        Dispose(false);
    }
}