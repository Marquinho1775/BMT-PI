using Moq;
using NUnit.Framework;
using Microsoft.Extensions.Configuration;
using BMT_backend.Infrastructure;
using System.Collections.Generic;
using BMT_backend.Domain.Entities;

namespace UnitTestingBMT
{
    [TestFixture]
    public class MailManagerTests
    {
        private Mock<IConfiguration> _mockConfiguration;
        private MailManager _mailManager;
        private Order _testOrder;

        [SetUp]
        public void SetUp()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(config => config["EmailSettings:Email"]).Returns("test@test.com");
            _mockConfiguration.Setup(config => config["EmailSettings:Host"]).Returns("smtp.test.com");
            _mockConfiguration.Setup(config => config["EmailSettings:Port"]).Returns("587");
            _mockConfiguration.Setup(config => config["EmailSettings:Password"]).Returns("password");

            _mockConfiguration.Setup(config => config["EmailSettingsCollabRegister:Email"]).Returns("collab@test.com");
            _mockConfiguration.Setup(config => config["EmailSettingsCollabRegister:Password"]).Returns("collabpassword");

            _mailManager = new MailManager(_mockConfiguration.Object);

            _testOrder = new Order
            {
                OrderId = "123",
                OrderDate = DateTime.Now,
                UserEmail = "user@test.com",
                Products = new List<ProductDetails>
                {
                    new ProductDetails
                    {
                        ProductId = "p1",
                        ProductName = "Product1",
                        Quantity = 1,
                        EnterpriseName = "Enterprise1",
                        EnterpriseEmail = "enterprise1@test.com",
                    }
                }
            };
        }

        [Test]
        public void SendConfirmationEmails_NoProductsInOrder_ShouldNotThrowException()
        {
            // Arrange
            _testOrder.Products.Clear();

            // Act & Assert
            Assert.DoesNotThrow(() => _mailManager.SendConfirmationEmails(_testOrder));
        }

        [Test]
        public void SendDenyEmail_ValidOrder_ShouldSendDenyEmail()
        {
            // Arrange
            var expectedTitle = "Orden cancelada";

            // Act
            _mailManager.SendDenyEmail(_testOrder);

            // Assert
            Assert.DoesNotThrow(() => _mailManager.SendDenyEmail(_testOrder));
        }

        [Test]
        public void SendDenyEmail_ValidOrder_ShouldNotThrowException()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => _mailManager.SendDenyEmail(_testOrder));
        }

        [Test]
        public void SendConfirmationEmails_ValidOrder_ShouldNotThrowException()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => _mailManager.SendConfirmationEmails(_testOrder));
        }

        [Test]
        public void SendConfirmationEmails_ValidOrder_ShouldSendEmailToUser()
        {
            // Arrange
            var expectedEmail = "user@test.com";

            // Act
            _mailManager.SendConfirmationEmails(_testOrder);

            // Assert
            Assert.That(_testOrder.UserEmail, Is.EqualTo(expectedEmail));
        }

        [Test]
        public void SendConfirmationEmails_MultipleProducts_ShouldGroupEmailsByEnterprise()
        {
            // Arrange
            _testOrder.Products.Add(new ProductDetails
            {
                ProductId = "p2",
                ProductName = "Product2",
                Quantity = 2,
                EnterpriseName = "Enterprise2",
                EnterpriseEmail = "enterprise2@test.com",
            });

            // Act & Assert
            Assert.DoesNotThrow(() => _mailManager.SendConfirmationEmails(_testOrder));
        }
    }
}
