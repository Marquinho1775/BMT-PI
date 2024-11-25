using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BMT_backend.Application.Interfaces;
using BMT_backend.Application.Services;
using BMT_backend.Domain.Entities;
using BMT_backend.Presentation.DTOs;
using BMT_backend.Presentation.Requests;

namespace BMT_backend.Tests.Services
{
    public class OrderServiceTests
    {
        private Mock<IOrderRepository> _orderRepositoryMock;
        private Mock<IProductRepository> _productRepositoryMock;
        private Mock<TagService> _tagServiceMock;
        private Mock<IImageFileService> _imageFileServiceMock;
        private Mock<DirectionService> _directionServiceMock;
        private ProductService _productService;
        private OrderService _orderService;

        [SetUp]
        public void Setup()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _productRepositoryMock = new Mock<IProductRepository>();
            _tagServiceMock = new Mock<TagService>(MockBehavior.Default, new Mock<ITagRepository>().Object);
            _imageFileServiceMock = new Mock<IImageFileService>();
            _directionServiceMock = new Mock<DirectionService>(MockBehavior.Default, new Mock<IDirectionRepository>().Object);

            _productService = new ProductService(_productRepositoryMock.Object, _tagServiceMock.Object, _imageFileServiceMock.Object);
            _orderService = new OrderService(_orderRepositoryMock.Object, _productService, _directionServiceMock.Object);
        }

        [Test]
        public async Task GetOrderReportsAsync_WhenStatusInicialIsNot0Or4_CallsFormatCanceledOrders()
        {
            // Arrange
            var reportRequest = new ReportRequest
            {
                FechaInicio = new DateTime(2023, 01, 01),
                FechaFin = new DateTime(2023, 12, 31),
                statusInicial = 5, // Value other than 0 or 4 to test the else block
                statusFinal = 6
            };

            var mockOrderDetails = new List<OrderDetails>
    {
        new OrderDetails
        {
            Order = new Order
            {
                NumOrder = "ORD123",
                OrderDate = new DateTime(2023, 02, 15),
                DeliveryDate = new DateTime(2023, 02, 20).ToString("yyyy-MM-dd"),
                Status = 5, // Canceled by user
                OrderCost = 5000,
                DeliveryFee = 500
            },
            Products = new List<ProductDetails>
            {
                new ProductDetails { EnterpriseName = "Empresa1" },
                new ProductDetails { EnterpriseName = "Empresa2" }
            }
        }
    };

            _orderRepositoryMock
                .Setup(repo => repo.GetOrderReportsAsync(It.IsAny<ReportRequest>()))
                .ReturnsAsync(mockOrderDetails);

            // Act
            var result = await _orderService.GetOrderReportsAsync(reportRequest);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].NumOrder, Is.EqualTo("ORD123"));
            Assert.That(result[0].CancelBy, Is.EqualTo("Cliente"));
            Assert.That(result[0].ProductCost, Is.EqualTo(5000));
            Assert.That(result[0].FeeCost, Is.EqualTo(500));
            Assert.That(result[0].TotalCost, Is.EqualTo(5500)); // TotalCost should include both ProductCost and FeeCost
        }
    }
}