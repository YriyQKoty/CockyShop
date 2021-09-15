using Humanizer;

namespace CockyShop.Models.App
{
    public class ProductStock
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        
        public Product Product { get; set; }
        
        public int StockId { get; set; }
        
        public Stock Stock { get; set; }
        
        public float Price { get; set; }
        
        public float QtyOnStock { get; set; }
    }
}