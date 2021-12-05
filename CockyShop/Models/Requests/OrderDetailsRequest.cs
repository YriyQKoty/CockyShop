using System.Collections.Generic;
using CockyShop.Models.DTO;

namespace CockyShop.Models.Requests
{
    public class OrderDetailsRequest
    {
        public ICollection<OrderedProductRequest> OrderedProductsRequests { get; set; }
    }
}