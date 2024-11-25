using BMT_backend.Application.Interfaces;
using BMT_backend.Application.Services;
using BMT_backend.Presentation.Requests;
using Moq;
using NUnit.Framework;

namespace BMT_backend.Tests.Application.Services
{
    [TestFixture]
    public class YearlyEarningsReportTests
    {
        private Mock<IEnterpriseRepository> _enterpriseRepositoryMock;
        private Mock<IProductRepository> _productRepositoryMock;
        private ProductService _productService;
        private EnterpriseService _enterpriseService;

        [SetUp]
        public void SetUp()
        {
            _enterpriseRepositoryMock = new Mock<IEnterpriseRepository>();
            _productRepositoryMock = new Mock<IProductRepository>();
            _enterpriseService = new EnterpriseService(_enterpriseRepositoryMock.Object, _productService);
        }

        [Test]
        public void YearlyEarningsReportValidation_ShouldReturnFalse_WhenEnterpriseIdsIsNullOrEmpty()
        {
            // Arrange
            var invalidRequest = new YearlyEarningsReportDataRequest
            {
                EnterpriseIds = "",
                Year = 2024
            };

            // Act
            var result = _enterpriseService.YearlyEarningsReportValidation(invalidRequest);

            // Assert
            Assert.That(result, Is.False, "YearlyEarningsReportValidation should return false when EnterpriseIds is null or empty.");
        }

        [Test]
        public void YearlyEarningsReportValidation_ShouldReturnFalse_WhenYearIsNegative()
        {
            // Arrange
            var invalidRequest = new YearlyEarningsReportDataRequest
            {
                EnterpriseIds = "123,456",
                Year = -1
            };

            // Act
            var result = _enterpriseService.YearlyEarningsReportValidation(invalidRequest);

            // Assert
            Assert.That(result, Is.False, "YearlyEarningsReportValidation should return false when Year is negative.");
        }

        [Test]
        public void YearlyEarningsReportValidation_ShouldReturnTrue_WhenRequestIsValid()
        {
            // Arrange
            var validRequest = new YearlyEarningsReportDataRequest
            {
                EnterpriseIds = "123,456",
                Year = 2024
            };

            // Act
            var result = _enterpriseService.YearlyEarningsReportValidation(validRequest);

            // Assert
            Assert.That(result, Is.True, "YearlyEarningsReportValidation should return true when the request is valid.");
        }
    }
}
