using AwesomeAssertions;
using Influunt.Scheduler.Abstractions.Services;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Influunt.Scheduler.Tests;

public class JobSchedulerIntegrationTests : SchedulerIntegrationTestBase
{
    protected override void ConfigureServices(IServiceCollection services, string connectionString)
    {
        // Register test jobs
        services.AddTransient<TestJobWithoutSettings>();
        services.AddTransient<TestJobWithSettings>();
        
        // Add scheduler module with hosted service registration
        services.AddJobSchedulerHost(options =>
        {
            options.ConnectionString = connectionString;
            options.ScanIntervalSeconds = 1; // Faster scanning for tests
            options.LockTimeoutSeconds = 5;
            options.HeartbeatIntervalSeconds = 2;
            options.MaxConcurrentJobs = 10;
        });
    }

    protected override async Task InitializeTestDataAsync()
    {
        // Reset static fields
        TestJobWithoutSettings.WasExecuted = false;
        TestJobWithSettings.WasExecuted = false;
        TestJobWithSettings.LastSettings = null;
        
        await base.InitializeTestDataAsync();
    }

    [Fact]
    public async Task Should_Register_Job_Without_Settings()
    {
        // Arrange
        var scheduler = GetRequiredService<IScheduler>();

        // Act
        await scheduler.RegisterJobAsync<TestJobWithoutSettings>("TestJobWithoutSettings", "Test job without settings");

        // Assert
        var jobs = await scheduler.GetRegisteredJobsAsync();
        jobs.Should().ContainSingle();
        var job = jobs.First();
        job.Id.Should().NotBeNullOrEmpty();
        job.Name.Should().Be("TestJobWithoutSettings");
        job.Description.Should().Be("Test job without settings");
        job.JobType.Should().Be(typeof(TestJobWithoutSettings).AssemblyQualifiedName);
        job.SettingsType.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task Should_Register_Job_With_Settings()
    {
        // Arrange
        var scheduler = GetRequiredService<IScheduler>();

        // Act
        await scheduler.RegisterJobAsync<TestJobWithSettings, TestJobSettings>("TestJobWithSettings", "Test job with settings");

        // Assert
        var jobs = await scheduler.GetRegisteredJobsAsync();
        jobs.Should().ContainSingle();
        var job = jobs.First();
        job.Id.Should().NotBeNullOrEmpty();
        job.Name.Should().Be("TestJobWithSettings");
        job.Description.Should().Be("Test job with settings");
        job.JobType.Should().Be(typeof(TestJobWithSettings).AssemblyQualifiedName);
        job.SettingsType.Should().Be(typeof(TestJobSettings).AssemblyQualifiedName);
    }

    [Fact]
    public async Task Should_Schedule_OneTime_Job_Without_Settings()
    {
        // Arrange
        var scheduler = GetRequiredService<IScheduler>();
        await scheduler.RegisterJobAsync<TestJobWithoutSettings>("TestJobWithoutSettings");

        // Act
        var triggerId = await scheduler.ScheduleOneTimeJobAsync<TestJobWithoutSettings>(DateTime.UtcNow.AddSeconds(1));

        // Assert
        triggerId.Should().NotBeNullOrEmpty();
        
        var triggers = await scheduler.GetScheduledTriggersAsync();
        triggers.Should().ContainSingle();
        var trigger = triggers.First();
        trigger.Id.Should().NotBeNullOrEmpty();
        trigger.TriggerId.Should().Be(triggerId);
        trigger.JobType.Should().Be(typeof(TestJobWithoutSettings).AssemblyQualifiedName);
        trigger.IsRecurring.Should().BeFalse();
        
        // Wait for job to execute
        await Task.Delay(3000);
        
        TestJobWithoutSettings.WasExecuted.Should().BeTrue();
        
        // Trigger should be removed after execution
        triggers = await scheduler.GetScheduledTriggersAsync();
        triggers.Should().BeEmpty();
    }

    [Fact]
    public async Task Should_Schedule_Recurring_Job_Without_Settings()
    {
        // Arrange
        var scheduler = GetRequiredService<IScheduler>();
        await scheduler.RegisterJobAsync<TestJobWithoutSettings>("TestJobWithoutSettings");

        // Act
        var triggerId = await scheduler.ScheduleRecurringJobAsync<TestJobWithoutSettings>("* * * * *"); // Every minute

        // Assert
        triggerId.Should().NotBeNullOrEmpty();
        
        var triggers = await scheduler.GetScheduledTriggersAsync();
        triggers.Should().ContainSingle();
        var trigger = triggers.First();
        trigger.Id.Should().NotBeNullOrEmpty();
        trigger.TriggerId.Should().Be(triggerId);
        trigger.JobType.Should().Be(typeof(TestJobWithoutSettings).AssemblyQualifiedName);
        trigger.IsRecurring.Should().BeTrue();
        trigger.CronExpression.Should().Be("* * * * *");
        
        await Task.Delay(TimeSpan.FromSeconds(70)); // Wait 70 seconds to ensure job runs (around next minute)
        
        TestJobWithoutSettings.WasExecuted.Should().BeTrue();
        
        // Trigger should still exist
        triggers = await scheduler.GetScheduledTriggersAsync();
        triggers.Should().ContainSingle();
    }

    [Fact]
    public async Task Should_Schedule_OneTime_Job_With_Settings()
    {
        // Arrange
        var scheduler = GetRequiredService<IScheduler>();
        await scheduler.RegisterJobAsync<TestJobWithSettings, TestJobSettings>("TestJobWithSettings");
        
        var settings = new TestJobSettings
        {
            Message = "Hello Test",
            Count = 42
        };

        // Act
        var triggerId = await scheduler.ScheduleOneTimeJobAsync<TestJobWithSettings, TestJobSettings>(
            settings, DateTime.UtcNow.AddSeconds(1));

        // Assert
        triggerId.Should().NotBeNullOrEmpty();
        
        // Wait for job to execute
        await Task.Delay(TimeSpan.FromSeconds(3));
        
        TestJobWithSettings.WasExecuted.Should().BeTrue();
        TestJobWithSettings.LastSettings.Should().NotBeNull();
        TestJobWithSettings.LastSettings!.Message.Should().Be("Hello Test");
        TestJobWithSettings.LastSettings!.Count.Should().Be(42);
        
        // Trigger should be removed after execution
        var triggers = await scheduler.GetScheduledTriggersAsync();
        triggers.Should().BeEmpty();
    }

    [Fact]
    public async Task Should_Unschedule_Job()
    {
        // Arrange
        var scheduler = GetRequiredService<IScheduler>();
        await scheduler.RegisterJobAsync<TestJobWithoutSettings>("TestJobWithoutSettings");
        var triggerId = await scheduler.ScheduleOneTimeJobAsync<TestJobWithoutSettings>(DateTime.UtcNow.AddMinutes(5));

        // Act
        await scheduler.UnScheduleJobAsync(triggerId);

        // Assert
        var triggers = await scheduler.GetScheduledTriggersAsync();
        triggers.Should().BeEmpty();
    }

    [Fact]
    public async Task Should_Not_Allow_Duplicate_Trigger_Execution()
    {
        // Arrange
        var scheduler = GetRequiredService<IScheduler>();
        await scheduler.RegisterJobAsync<TestJobWithoutSettings>("TestJobWithoutSettings", allowConcurrentExecution: false);
        
        var runAt = DateTime.UtcNow.AddSeconds(1);
        await scheduler.ScheduleOneTimeJobAsync<TestJobWithoutSettings>(runAt, "test-trigger");
        var duplicateTriggerId = () => scheduler.ScheduleOneTimeJobAsync<TestJobWithoutSettings>(runAt.AddSeconds(10), "test-trigger");

        var exception = await Assert.ThrowsAsync<MongoWriteException>(duplicateTriggerId);
        exception.WriteError.Category.Should().Be(ServerErrorCategory.DuplicateKey);
        exception.WriteError.Code.Should().Be(11000);
    }

    [Fact]
    public async Task Should_Handle_Multiple_Concurrent_Jobs()
    {
        // Arrange
        var scheduler = GetRequiredService<IScheduler>();
        await scheduler.RegisterJobAsync<TestJobWithoutSettings>("TestJobWithoutSettings", allowConcurrentExecution: true);
        
        // Act
        var triggerId1 = await scheduler.ScheduleOneTimeJobAsync<TestJobWithoutSettings>(DateTime.UtcNow.AddSeconds(1), "trigger1");
        var triggerId2 = await scheduler.ScheduleOneTimeJobAsync<TestJobWithoutSettings>(DateTime.UtcNow.AddSeconds(1), "trigger2");
        var triggerId3 = await scheduler.ScheduleOneTimeJobAsync<TestJobWithoutSettings>(DateTime.UtcNow.AddSeconds(1), "trigger3");

        // Assert
        var triggers = await scheduler.GetScheduledTriggersAsync();
        triggers.Should().HaveCount(3);
        
        // Wait for execution
        await Task.Delay(TimeSpan.FromSeconds(4));
        
        TestJobWithoutSettings.WasExecuted.Should().BeTrue();
        TestJobWithoutSettings.ExecutionCount.Should().Be(3);
        
        // All triggers should be removed after execution
        triggers = await scheduler.GetScheduledTriggersAsync();
        triggers.Should().BeEmpty();
    }
}