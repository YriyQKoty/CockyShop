namespace CockyShop.Models.DTO
{
    public class OrderedProductDto
    {
        public int ProductStockId { get; set; }
        
        public int OrderDetailsId { get; set; }
        
        public int Quantity { get; set; }

        public float PricePerUnit { get; set; }
    }
}