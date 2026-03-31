namespace Influunt.Scheduler.Abstractions.Models;

public enum ExecutionStatus
{
    Pending,    // Awaiting execution (lock not acquired)
    Running,    // Running (lock acquired)
    Completed,  // Successfully completed
    Failed      // Failed
}