using BMT_backend.Domain.Entities;

namespace BMT_backend.Domain.Entities
{
    public class Order
    {
        public string? OrderId { get; set; }
        public string UserId { get; set; }
        public string DirectionId { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime? OrderDate { get; set; }
        public string DeliveryDate { get; set; }
        public double? OrderCost { get; set; }
        public double? Weight { get; set; }
        public double? DeliveryFee { get; set; }
        public int Status { get; set; }
        /*
         * 0 No confirmado
         * 1 Confirmado
         * 2 Listo para envío
         * 3 Shipping
         * 4 Terminado
         * 5 Cancelado user
         * 6 Cancelado dv
         */ 
    }
}
