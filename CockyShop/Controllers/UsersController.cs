using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CockyShop.Exceptions;
using CockyShop.Infrastucture;
using CockyShop.Models.DTO;
using CockyShop.Models.Identity;
using CockyShop.Models.Requests;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CockyShop.Controllers
{
    public class UsersController : AppBaseController
    {
        private SignInManager<AppUser> _signInManager;
        private UserManager<AppUser> _userManager;
        private AppDbContext _appDbContext;
        private IMapper _mapper;

        public UsersController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, AppDbContext appDbContext, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _appDbContext = appDbContext;
            _mapper = mapper;
        }
        
        [HttpPost("login")]
        public async Task<ActionResult<LoggedUserDto>> Login([FromBody] LoggedUserRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                throw new EntityNotFoundException($"User with such email {request.Email} was not found!");
            }

            var checkPwd = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

            if (!checkPwd.Succeeded)
            {
                throw new InvalidInputException($"Incorrect password!");
            }
            
            return Ok(new LoggedUserDto()
            {
                User = new AppUserDto()
                {
                    Email = user.Email,
                    UserName = user.UserName,
                    Orders = new List<OrderDto>()
                },
                JwtToken = String.Empty
            });
        }

        [HttpPost("register")]
        public async Task<ActionResult<AppUserDto>> Register([FromBody] RegisterUserRequest request)
        {
            var userNameDuplicate = await _userManager.FindByNameAsync(request.UserName);

            if (userNameDuplicate != null)
            {
                throw new DomainException($"User with such a name {request.UserName} already exists!");
            }
            
            var user = new AppUser()
            {
                UserName = request.UserName,
                Email = request.Email
                
            };

            var identityUser = await _userManager.CreateAsync(user, request.Password);
            if (!identityUser.Succeeded)
            {
                var err = identityUser.Errors.First();
                throw new DomainException($"{err.Description}, {err.Code}");
            }

            return Ok(new AppUserDto()
            {
                Email = user.Email,
                UserName = user.UserName,
                Orders = new List<OrderDto>()
            });
        }

        [HttpGet]
        public async Task<ActionResult<List<AppUserDto>>> GetUsers()
        {
            return Ok(await _appDbContext.Users.Select(ap => new AppUserDto()
            {
                Email = ap.Email,
                UserName = ap.UserName
            }).ToListAsync());
        }

        [HttpGet("email")]
        public async Task<ActionResult<AppUserDto>> GetUserByEmail([FromQuery] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                throw new EntityNotFoundException($"User with such email {email} was not found!");
            }

            user.Orders = await _appDbContext.Orders.Where(o => o.UserId == user.Id).ToListAsync();

            return Ok(new AppUserDto()
            {
                Email = user.Email,
                UserName = user.UserName,
                Orders = _mapper.Map<List<OrderDto>>(user.Orders)
            });
        }
    }
}