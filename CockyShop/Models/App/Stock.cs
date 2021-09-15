using System.Collections.Generic;

namespace CockyShop.Models.App
{
    public class Stock
    {
        public int Id { get; set; }
        
        public City City { get; set; }
        
        public ICollection<ProductStock> ProductStocks { get; set; } = new List<ProductStock>();
    }
}