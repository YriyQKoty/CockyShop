namespace CockyShop.Models.Requests
{
    public class OrderedProductRequest
    {
        public int ProductStockId { get; set; }

        public int Quantity { get; set; }
    }
}