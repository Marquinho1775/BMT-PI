namespace BMT_backend.Presentation.DTOs
{
    public class ReportDto
    {
        public string NumOrder { get; set; }
        public string Enterprises { get; set; }
        public int ItemsCount { get; set; }
        public DateTime DateOfCreation { get; set; }

        // Columna cambiante
        public string? DateOfDelivery { get; set; }
        public string? DateOfCancelation { get; set; }

        // Columna cambiante
        public string? DateReceived { get; set; }
        public string? CancelBy { get; set; }
        public string? Status { get; set; }

        public double ProductCost { get; set; }
        public double FeeCost { get; set; }
        public double TotalCost { get; set; }
    }
}