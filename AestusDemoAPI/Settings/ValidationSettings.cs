namespace AestusDemoAPI.Settings
{
    public class ValidationSettings
    {
        public decimal InvalidAmount { get; set; }
        public int BatchDelayMs { get; set; }
        public int AnomalyCount { get; set; }
        public int StandardDeviation { get; set; }
        public List<string> Locations { get; set; } = [];
    }
}
