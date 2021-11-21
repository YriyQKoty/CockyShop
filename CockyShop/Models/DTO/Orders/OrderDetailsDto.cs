using System;
using System.Collections.Generic;
using CockyShop.Models.App;
using CockyShop.Models.Enums;

namespace CockyShop.Models.DTO
{
    public class OrderDetailsDto
    {
        public int Id { get; set; }
        
        public DateTime DateOrdered { get; set; }
        
        public TimeSpan EstimatedTime { get; set; }
        
        public OrderStatusDto Status { get; set; }

        public ICollection<OrderedProductDto> OrderedProductsDtos { get; set; }
    }
}