namespace Influunt.Scheduler;

public class JobTrigger
{
    public string Id { get; set; }
    public string TriggerId { get; set; }
    public string JobType { get; set; }
    public DateTime NextRunTime { get; set; }
    public bool IsRecurring => !string.IsNullOrEmpty(CronExpression);
    public string? CronExpression { get; set; }
    public string? SettingsJson { get; set; }
}