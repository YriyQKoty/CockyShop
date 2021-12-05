using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CockyShop.Models.App
{
    public class Order
    {
        public int Id { get; set; }
        
        [ForeignKey("AppUserId")]
        public string UserId { get; set; }
        
        public int OrderDetailsId { get; set; }

        public OrderDetails OrderDetails { get; set; }

    }
}