namespace BMT_backend.Application.Services;
using System;
public static class DistanceCalculator
{
    public static double CalcularDistancia(double lat1, double lon1, double lat2 = 9.937722114664625, double lon2 = -84.05251479782451)
    {
        const double RadioTierra = 6371e3;
        double φ1 = ConvertirGradosARadianes(lat1);
        double φ2 = ConvertirGradosARadianes(lat2);
        double Δφ = ConvertirGradosARadianes(lat2 - lat1);
        double Δλ = ConvertirGradosARadianes(lon2 - lon1);

        double a = Math.Sin(Δφ / 2) * Math.Sin(Δφ / 2) +
                   Math.Cos(φ1) * Math.Cos(φ2) *
                   Math.Sin(Δλ / 2) * Math.Sin(Δλ / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        double distancia = RadioTierra * c;

        return distancia / 1000.0;
    }

    private static double ConvertirGradosARadianes(double grados)
    {
        return grados * (Math.PI / 180);
    }
}