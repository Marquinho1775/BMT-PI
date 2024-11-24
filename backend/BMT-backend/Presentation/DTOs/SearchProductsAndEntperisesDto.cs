using BMT_backend.Domain.Entities;

namespace BMT_backend.Presentation.DTOs
{
    public class SearchProductsAndEntperisesDto
    {
        public List<Product> Products { get; set; }
        public List<Enterprise> Enterprises { get; set; }
    }
}
