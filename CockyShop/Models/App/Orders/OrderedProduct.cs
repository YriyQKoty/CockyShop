namespace CockyShop.Models.App
{
    
    public class OrderedProduct
    {
        public int Id { get; set; }
        
        public int ProductStockId { get; set; }
        
        public ProductStock ProductStock { get; set; }
        
        public int OrderDetailsId { get; set; }
        
        public OrderDetails OrderDetails { get; set; }
        
        public int Quantity { get; set; }

        public float PricePerUnit { get; set; }
    }
}