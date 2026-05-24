namespace Influunt.Scheduler.Abstractions.Models;
public interface IJob
{
    Task ExecuteAsync(CancellationToken cancellationToken);
}

public interface IJob<TSettings>
{
    Task ExecuteAsync(TSettings settings, CancellationToken cancellationToken);
}