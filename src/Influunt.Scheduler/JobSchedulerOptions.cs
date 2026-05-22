namespace Influunt.Scheduler;

    public class JobSchedulerOptions
    {
        /// <summary>MongoDB connection string.</summary>
        public string ConnectionString { get; set; } = "";

        /// <summary>Trigger scan interval (seconds).</summary>
        public int ScanIntervalSeconds { get; set; } = 60;

        /// <summary>Lock timeout duration (seconds).</summary>
        public int LockTimeoutSeconds { get; set; } = 300;

        /// <summary>Heartbeat update interval (seconds).</summary>
        public int HeartbeatIntervalSeconds { get; set; } = 60;

        /// <summary>Maximum number of concurrently running jobs.</summary>
        public int MaxConcurrentJobs { get; set; } = 10;
    }
