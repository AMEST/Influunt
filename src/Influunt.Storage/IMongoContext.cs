using MongoDB.Driver;

namespace Influunt.Storage;

/// <summary>
/// Интерфейс для работы с MongoDB контекстом
/// </summary>
public interface IMongoContext
{
    /// <summary>
    /// Получает базу данных MongoDB
    /// </summary>
    IMongoDatabase Database { get; }
    
    /// <summary>
    /// Получает коллекцию указанного типа
    /// </summary>
    /// <typeparam name="T">Тип документов коллекции</typeparam>
    /// <param name="collectionName">Имя коллекции (если null, будет использовано имя типа)</param>
    /// <returns>Коллекция MongoDB</returns>
    IMongoCollection<T> GetCollection<T>(string collectionName = null);
    
    /// <summary>
    /// Освобождает ресурсы
    /// </summary>
    void Dispose();
}