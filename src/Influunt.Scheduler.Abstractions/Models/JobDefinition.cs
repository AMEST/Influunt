namespace Influunt.Scheduler.Abstractions.Models;

public class JobDefinition
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string JobType { get; set; }
    public string SettingsType { get; set; }
    public bool AllowConcurrentExecution { get; set; }
}