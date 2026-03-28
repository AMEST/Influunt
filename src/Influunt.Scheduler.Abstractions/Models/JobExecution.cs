namespace Influunt.Scheduler.Abstractions.Models;

public class JobExecution
{
    public string Id { get; set; }
    public string TriggerId { get; set; }
    public string JobType { get; set; }
    public ExecutionStatus Status { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string Result { get; set; }
    public string LockId { get; set; }
    public DateTime LockExpiry { get; set; }
    public DateTime LastHeartbeat { get; set; }
}