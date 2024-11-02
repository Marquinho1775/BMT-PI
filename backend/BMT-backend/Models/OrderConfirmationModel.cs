﻿using System;
using System.Collections.Generic;

namespace BMT_backend.Models
{
    public class OrderConfirmationModel
    {
        public string OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal OrderCost { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal Weight { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string Direction { get; set; } = null!;
        public string UserEmail { get; set; } = null!;
        public string? OtherSigns { get; set; }
        public string Coordinates { get; set; } = null!;
        /*
         * 0 No confirmado
         * 1 Confirmado
         * 2 Listo para envío
         * 3 Shipping
         * 4 Terminado
         * 5 Cancelado
         */
        public int Status { get; set; }
        public List<ProductDetails> Products { get; set; } = new List<ProductDetails>();
    }

    public class ProductDetails
    {
        public string ProductId { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public int Quantity { get; set; }
        public int ProductsCost { get; set; }
        public string EnterpriseName { get; set; } = null!;
        public string EnterpriseEmail { get; set; } = null;
    }
}