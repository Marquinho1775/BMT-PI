using BMT_backend.Application.Interfaces;
using BMT_backend.Domain.Entities;
using BMT_backend.Presentation.DTOs;
using System.Globalization;

namespace BMT_backend.Application.Queries
{
    public class SearchProductsAndEnterprisesQuerie
    {
        private readonly IProductRepository _productRepository;
        private readonly IEnterpriseRepository _enterpriseRepository;
        private static readonly HashSet<string> StopWords = new HashSet<string>
        {
            "de", "para", "del", "el", "la", "los", "las", "y", "o", "un", "una", "unos", "unas", "en", "por", "con", "a", "al", 
            "se", "que", "es", "su", "lo", "como", "más", "pero", "sus", "le", "ya", "si", "porque", "muy", "sin", "sobre"
        };

        public SearchProductsAndEnterprisesQuerie(IProductRepository productRepository, IEnterpriseRepository enterpriseRepository)
        {
            _productRepository = productRepository;
            _enterpriseRepository = enterpriseRepository;
        }

        public async Task<SearchProductsAndEntperisesDto> SearchProductsAndEnterprisesAsync(string userInput)
        {
            var products = await SearchProductsAsync(userInput);
            var enterprises = await SearchEnterprisesAsync(userInput);
            return new SearchProductsAndEntperisesDto
            {
                Products = products,
                Enterprises = enterprises
            };
        }

        private async Task<List<Product>> SearchProductsAsync(string userInput)
        {
            var isAboutTerm = BuildIsAboutSearchTerm(userInput);
            var productsId = await _productRepository.SearchProductsIdAsync(isAboutTerm);
            var products = new List<Product>();
            foreach (var productId in productsId)
            {
                var product = await _productRepository.GetProductDetailsByIdAsync(productId);
                products.Add(product);
            }
            return products;
        }

        private async Task<List<Enterprise>> SearchEnterprisesAsync(string userInput)
        {
            var isAboutTerm = BuildIsAboutSearchTerm(userInput);
            var enterprises = new List<Enterprise>();
            var enterprisesId = await _enterpriseRepository.SearchEnterprisesIdAsync(isAboutTerm);
            foreach (var enterpriseId in enterprisesId)
            {
                var enterprise = await _enterpriseRepository.GetEnterpriseByIdAsync(enterpriseId);
                enterprises.Add(enterprise);
            }
            return enterprises;
        }

        private static string BuildIsAboutSearchTerm(string userInput)
        {
            if (string.IsNullOrWhiteSpace(userInput))
                return "*";
            var cleanedInput = new string(userInput.ToLower().Where(c => !char.IsPunctuation(c)).ToArray());
            var words = cleanedInput.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var relevantWords = words.Where(w => !StopWords.Contains(w)).ToList();
            if (relevantWords.Count == 0)
                return "*";
            var totalWords = relevantWords.Count;
            var searchTerms = new List<string>();
            for (int i = 0; i < totalWords; i++)
            {
                var word = relevantWords[i];
                double weight = 1.0 - (i * (0.1));
                if (weight < 0.1) weight = 0.1;
                searchTerms.Add($"\"{word}\" weight({weight.ToString("0.0", CultureInfo.InvariantCulture)})");
            }
            var isAboutTerm = $"ISABOUT({string.Join(", ", searchTerms)})";
            return isAboutTerm;
        }
    }
}
