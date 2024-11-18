namespace BMT_backend.Presentation.Requests
{
    public class AddEntrepreneurToEnterpriseRequest
    {
        public string EntrepreneurIdentification { get; set; }
        public string EnterpriseIdentification { get; set; }
        public bool IsAdmin { get; set; }
    }
}
