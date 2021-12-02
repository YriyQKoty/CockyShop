using System.Collections.Generic;
using System.Threading.Tasks;
using CockyShop.Models.Identity;
using CockyShop.Models.Requests;

namespace CockyShop.Services
{
    public interface IUserService
    {
        Task<List<AppUser>> GetAllUsers();
        Task<AppRole> FindRoleByNameAsync(string roleName);
        Task<AppUser> FindUserByEmailAsync(string email);
        
        Task<AppUser> FindUserByUserIdAsync(string userId);
        Task ValidateDuplicatedUserAsync(RegisterUserRequest request);

        Task<ICollection<string>> GetAllUserRolesAsync(AppUser user);

        Task AddRoleToUserAsync(AppUser user, string role);

        Task CreateUserAsync(AppUser user, string password);

        Task DeleteUserAsync(AppUser user);
    }
}