using System.Collections.Generic;
using CockyShop.Models.App;

namespace CockyShop.Models.DTO
{
    public class AppUserDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        
        public ICollection<OrderDto> Orders { get; set; }
    }
}