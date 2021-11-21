using System;
using System.Collections;
using System.Collections.Generic;
using CockyShop.Models.Enums;

namespace CockyShop.Models.App
{
    public class OrderDetails
    {
        public int Id { get; set; }
        
        public int OrderId { get; set; }
        
        public Order Order { get; set; }

        public DateTime DateOrdered { get; set; }
        
        public TimeSpan EstimatedTime { get; set; }
        
        public OrderStatus Status { get; set; }

        public ICollection<OrderedProduct> OrderedProducts { get; set; } = new List<OrderedProduct>();
        
    }
}