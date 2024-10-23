using System;
using System.Collections.Generic;

namespace BMT_backend.Models
{
    public class OrderModel
    {
        public string OrderId { get; set; } = null!;
        public DateTime OrderDate { get; set; }
        public decimal OrderCost { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal Weight { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string DirectionNum { get; set; }
        public int Status { get; set; }
        public List<ProductDetails> Products { get; set; } = new List<ProductDetails>();
    }

    public class ProductDetails
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public string EntrepreneurName { get; set; }
    }
}