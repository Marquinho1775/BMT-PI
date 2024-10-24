using System;
using System.Collections.Generic;

namespace BMT_backend.Models
{
    public class OrderModel
    {
        public string OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal OrderCost { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal Weight { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string DirectionName { get; set; } = null!;
        public string Province { get; set; } = null!;
        public string Canton { get; set; } = null!;
        public string District { get; set; } = null!;
        public string? OtherSigns { get; set; }
        public string Coordinates { get; set; } = null!;
        public int Status { get; set; }
        public List<ProductDetails> Products { get; set; } = new List<ProductDetails>();
    }

    public class ProductDetails
    {
        public string ProductId { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public int Quantity { get; set; }
        public string EnterpriseName { get; set; } = null!;
}