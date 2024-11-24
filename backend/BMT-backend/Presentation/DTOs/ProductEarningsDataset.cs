namespace BMT_backend.Presentation.DTOs
{
    public class ProductEarningsDataset
    {
        public string ProductLabel { get; set; }
        public List<double> EarningsPerMonth { get; set; } = new List<double>(12);
    }
}
