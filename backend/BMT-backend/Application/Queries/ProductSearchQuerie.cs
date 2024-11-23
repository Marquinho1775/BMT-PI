using BMT_backend.Application.Interfaces;

namespace BMT_backend.Application.Queries
{
    public class ProductSearchQuerie
    {
        private readonly IProductRepository _productRepository;

        public ProductSearchQuerie(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
    }
}
