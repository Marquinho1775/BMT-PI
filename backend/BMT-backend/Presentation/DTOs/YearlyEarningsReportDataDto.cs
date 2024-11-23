namespace BMT_backend.Presentation.DTOs
{
    public class YearlyEarningsReportDataDto
    {
        public string EnterpriseName { get; set; }
        public string MonthName { get; set; }
        public decimal TotalPurchase { get; set; }
        public decimal TotalDelivery { get; set; }
        public decimal TotalCost { get; set; }
    }
}
