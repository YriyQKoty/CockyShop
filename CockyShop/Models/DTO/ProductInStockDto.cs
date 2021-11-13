namespace CockyShop.Models.DTO
{
    public class ProductInStockDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CityId { get; set; }
        public string CityName { get; set; }
        public float Price { get; set; }
        
        public int QtyOnStock { get; set; }
        public bool Available { get; set; }
    }
}