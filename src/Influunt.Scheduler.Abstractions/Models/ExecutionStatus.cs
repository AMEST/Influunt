namespace Influunt.Scheduler.Abstractions.Models;

public enum ExecutionStatus
{
    Pending,    // Ожидает выполнения (блокировка не получена)
    Running,    // Выполняется (блокировка получена)
    Completed,  // Успешно завершено
    Failed      // Завершено с ошибкой
}