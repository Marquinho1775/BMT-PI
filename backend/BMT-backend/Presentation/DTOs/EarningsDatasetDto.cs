namespace BMT_backend.Presentation.DTOs
{
    public class EarningsDatasetDto
    {
        public string Label { get; set; }
        public List<double> EarningsPerMonth { get; set; } = new List<double>(12);
    }
}
