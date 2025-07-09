namespace AestusDemoAPI.Settings
{
    public class ValidationSettings
    {
        public double MaxAmount { get; set; }
        public int AnomalyCount { get; set; }
        public List<string> Locations { get; set; } = [];
    }
}
