using System.Collections.Generic;
using CockyShop.Models.DTO;

namespace CockyShop.Models.Requests
{
    public class OrderRequest
    {
        public string UserEmail { get; set; }
        
        public OrderDetailsRequest OrderDetailsRequest { get; set; }
    }
}