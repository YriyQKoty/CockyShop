namespace CockyShop.Models.DTO
{
    public class LoggedUserDto
    {
        public AppUserDto User { get; init; }
        public string JwtToken { get; init; }
    }
}