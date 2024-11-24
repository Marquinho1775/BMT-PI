namespace BMT_backend.Presentation.Requests
{
    public class ReportRequest
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        public int statusInicial { get; set; }
        public int statusFinal { get; set; }

        public string? EnterpriseId { get; set; }
        public string? UserId { get; set; }
    }
}
