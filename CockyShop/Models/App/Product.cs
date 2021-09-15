using System.Collections;
using System.Collections.Generic;

namespace CockyShop.Models.App
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<ProductStock> ProductStocks { get; set; } = new List<ProductStock>();
    }
}