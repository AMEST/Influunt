using Cronos;

namespace Influunt.Scheduler;

public static class CronHelper
{
    public static DateTime? GetNextOccurrence(string cronExpression, DateTime currentTime)
    {
        CronExpression expression = CronExpression.Parse(cronExpression);
        return expression.GetNextOccurrence(currentTime);
    }
}
