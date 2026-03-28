namespace Influunt.Scheduler;

    public class JobSchedulerOptions
    {
        /// <summary>Строка подключения к MongoDB.</summary>
        public string ConnectionString { get; set; } = "mongodb://localhost:27017/JobScheduler";

        /// <summary>Интервал сканирования триггеров (сек).</summary>
        public int ScanIntervalSeconds { get; set; } = 60;

        /// <summary>Время жизни блокировки (сек).</summary>
        public int LockTimeoutSeconds { get; set; } = 300;

        /// <summary>Интервал обновления heartbeat (сек).</summary>
        public int HeartbeatIntervalSeconds { get; set; } = 60;

        /// <summary>Максимальное количество одновременно выполняемых заданий.</summary>
        public int MaxConcurrentJobs { get; set; } = 10;
    }