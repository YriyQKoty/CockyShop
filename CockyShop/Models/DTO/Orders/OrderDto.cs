using System.Collections.Generic;

namespace CockyShop.Models.DTO
{
    public class OrderDto
    {
        public int Id { get; set; }
        
        public string UserId { get; set; }
        
        public OrderDetailsDto OrderDetailsDto { get; set; }
    }
}