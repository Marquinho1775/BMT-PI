namespace BMT_backend.Domain.Entities
{
    public class CreditCard
    {
        public string Id { get; set; } = null!;
        public string UserID { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public string MagicNumber { get; set; }
        public string DateVenc { get; set; }
    }
}
