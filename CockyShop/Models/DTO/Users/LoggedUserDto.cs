using System.Collections.Generic;
using CockyShop.Models.Identity;

namespace CockyShop.Models.DTO
{
    public class LoggedUserDto
    {
        public AppUserDto User { get; init; }
        public string JwtToken { get; init; }
        
        public ICollection<string> Roles { get; init; }
    }
}