namespace AestusDemoAPI.Validation
{
    public static class TransactionAnomalyMessages
    {
        public const string ExpectedAmount = "Normalna potrošnja.";
        public const string FrequencySpike = "Otkriven nagli porast frekvencije transakcija.";
        public const string IQRAnomaly = "Otkrivena anomalija unutar interkvartilnog raspona.";
        public const string UnexpectedAmount = "Neočekivan iznos transakcije.";
        public const string UnexpectedLocation = "Neočekivana lokacija.";
    }
}
