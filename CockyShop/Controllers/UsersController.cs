using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CockyShop.Exceptions;
using CockyShop.Infrastucture;
using CockyShop.Models.DTO;
using CockyShop.Models.Identity;
using CockyShop.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace CockyShop.Controllers
{
    public class UsersController : AppBaseController
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UsersController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, 
            AppDbContext appDbContext, IMapper mapper, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _appDbContext = appDbContext;
            _mapper = mapper;
            _configuration = configuration;
        }
        
        [AllowAnonymous]
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

            var token = await GenerateJwtToken(request.Email, user);

            return Ok(new LoggedUserDto()
            {
                User = new AppUserDto()
                {
                    Email = user.Email,
                    UserName = user.UserName,
                    Orders = new List<OrderDto>()
                },
                JwtToken = token
            });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<AppUserDto>> Register([FromBody] RegisterUserRequest request)
        {
            await ValidateDuplicatedUser(request);

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

            await _userManager.AddToRoleAsync(user, "CUSTOMER");

            return Ok(new AppUserDto()
            {
                Email = user.Email,
                UserName = user.UserName,
                Orders = new List<OrderDto>()
            });
        }

        private async Task ValidateDuplicatedUser(RegisterUserRequest request)
        {
            var userNameDuplicate = await _userManager.FindByNameAsync(request.UserName);

            if (userNameDuplicate != null)
            {
                throw new DomainException($"User with such a name {request.UserName} already exists!");
            }
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
        [Authorize]
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
        
        
        private async Task<string> GenerateJwtToken(string email, AppUser user)
        {
           
            var userRoles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            };
            
            claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:JwtKey"]));

            var str = _configuration["Jwt:JwtExpireDays"];
            var value = Convert.ToDouble(str);
            var expires = DateTime.Now.AddDays(value);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Jwt:JwtIssuer"],
                _configuration["Jwt:JwtIssuer"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}