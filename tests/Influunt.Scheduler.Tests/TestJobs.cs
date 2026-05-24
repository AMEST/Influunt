using Influunt.Scheduler.Abstractions.Models;

namespace Influunt.Scheduler.Tests;

public class TestJobWithoutSettings : IJob
{
    public static int ExecutionCount = 0;
    public static bool WasExecuted { get; set; }
    
    public Task ExecuteAsync(CancellationToken cancellationToken)
    {
        WasExecuted = true;
        Interlocked.Increment(ref ExecutionCount);
        return Task.CompletedTask;
    }
}

public class TestJobWithSettings : IJob<TestJobSettings>
{
    public static int ExecutionCount = 0;
    public static TestJobSettings? LastSettings { get; set; }
    public static bool WasExecuted { get; set; }
    
    public Task ExecuteAsync(TestJobSettings settings, CancellationToken cancellationToken)
    {
        LastSettings = settings;
        WasExecuted = true;
        Interlocked.Increment(ref ExecutionCount);
        return Task.CompletedTask;
    }
}

public class TestJobSettings
{
    public string Message { get; set; } = string.Empty;
    public int Count { get; set; }
}