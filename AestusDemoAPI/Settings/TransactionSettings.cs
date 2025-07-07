namespace AestusDemoAPI.Settings
{
    public record TransactionSettings
    {
        public int BatchSize { get; set; }
        public int BatchDelayMs { get; set; }
        public int BatchTimeoutSeconds { get; set; }
    }
}
