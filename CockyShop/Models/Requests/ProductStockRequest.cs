namespace CockyShop.Models.Requests
{
    public class ProductStockRequest
    {
        public int CityId { get; set; }
        
        public int ProductId { get; set; }
        public float Price { get; set; }
        
        public float QtyOnStock { get; set; }
        
    }
}