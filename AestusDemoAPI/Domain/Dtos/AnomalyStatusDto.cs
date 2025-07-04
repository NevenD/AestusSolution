namespace AestusDemoAPI.Domain.Dtos
{
    public record AnomalyStatusDto
    {
        public bool IsSuspicious { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
