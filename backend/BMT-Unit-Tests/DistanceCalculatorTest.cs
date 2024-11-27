using NUnit.Framework;
using BMT_backend.Application.Services;

namespace BMT_backend.Tests.Application.Services
{
    [TestFixture]
    public class DistanceCalculatorTest
    {
        [Test]
        public void CalculateDistance_ShouldReturn26_24Km_ForSpecifiedCoordinates()
        {
            // Arrange
            double lat1 = 9.9378881;
            double lon1 = -84.0520808;
            double lat2 = 10.0277764;
            double lon2 = -84.2735830;
            double expectedDistance = 26.24; // en kilómetros
            double tolerance = 0.1; // tolerancia de 0.1 km

            // Act
            double calculatedDistance = DistanceCalculator.CalcularDistancia(lat1, lon1, lat2, lon2);

            // Assert
            Assert.That(calculatedDistance, Is.EqualTo(expectedDistance).Within(tolerance),
                $"La distancia calculada debería estar cerca de {expectedDistance} km.");
        }

        [Test]
        public void CalculateDistance_ShouldReturn1_27Km_ForAnotherSpecifiedCoordinates()
        {
            // Arrange
            double lat1 = 9.9378775;
            double lon1 = -84.0520647;
            double lat2 = 9.9388387;
            double lon2 = -84.0405071;
            double expectedDistance = 1.27; // en kilómetros
            double tolerance = 0.05; // tolerancia de 0.05 km

            // Act
            double calculatedDistance = DistanceCalculator.CalcularDistancia(lat1, lon1, lat2, lon2);

            // Assert
            Assert.That(calculatedDistance, Is.EqualTo(expectedDistance).Within(tolerance),
                $"La distancia calculada debería estar cerca de {expectedDistance} km.");
        }

        [Test]
        public void CalculateDistance_ShouldReturn0Km_ForIdenticalCoordinates()
        {
            // Arrange
            double lat1 = 9.9378881;
            double lon1 = -84.0520808;
            double lat2 = 9.9378881;
            double lon2 = -84.0520808;
            double expectedDistance = 0.0; // en kilómetros
            double tolerance = 0.0001; // tolerancia mínima

            // Act
            double calculatedDistance = DistanceCalculator.CalcularDistancia(lat1, lon1, lat2, lon2);

            // Assert
            Assert.That(calculatedDistance, Is.EqualTo(expectedDistance).Within(tolerance),
                "La distancia calculada entre puntos idénticos debería ser 0 km.");
        }
    }
}
