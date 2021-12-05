using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CockyShop.Exceptions;
using CockyShop.Infrastucture;
using CockyShop.Models.Identity;
using CockyShop.Models.Requests;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CockyShop.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _appDbContext;

        public UserService(UserManager<AppUser> userManager, AppDbContext appDbContext)
        {
            _userManager = userManager;
            _appDbContext = appDbContext;
        }


        public async Task<List<AppUser>> GetAllUsers()
        {
            return (await _appDbContext.Users.Select(u => u).ToListAsync());
        }

        public async  Task<AppRole> FindRoleByNameAsync(string roleName)
        {
            var role = await _appDbContext.Roles.SingleOrDefaultAsync(r => r.Name == roleName);

            if (role == null)
            {
                throw new EntityNotFoundException($"Role with such name {roleName} was not found!");
            }

            return role;
        }

        public async Task<AppUser> FindUserByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                throw new EntityNotFoundException($"User with such email {email} was not found!");
            }

            return user;
        }

        public async Task<AppUser> FindUserByUserIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                throw new EntityNotFoundException($"User with such User GeneralProductId {userId} was not found!");
            }

            return user;
        }


        public async Task ValidateDuplicatedUserAsync(RegisterUserRequest request)
        {
            var userNameDuplicate = await _userManager.FindByNameAsync(request.UserName);

            if (userNameDuplicate != null)
            {
                throw new DomainException($"User with such a name {request.UserName} already exists!");
            }
        }

        public async Task<ICollection<string>> GetAllUserRolesAsync(AppUser user)
        {
           return (await _userManager.GetRolesAsync(user)).ToList();
        }

        public async Task AddRoleToUserAsync(AppUser user, string role)
        {
            if (! (await _userManager.IsInRoleAsync(user, role)))
            {
                await _userManager.AddToRoleAsync(user,role);
            }
        }

        public async Task CreateUserAsync(AppUser user, string password)
        {
            var identityUser = await _userManager.CreateAsync(user, password);
            if (!identityUser.Succeeded)
            {
                var err = identityUser.Errors.First();
                throw new DomainException($"{err.Description}, {err.Code}");
            }
        
        }

        public async Task DeleteUserAsync(AppUser user)
        {
            await _userManager.DeleteAsync(user);
        }
    }
}