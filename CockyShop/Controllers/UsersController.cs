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
using CockyShop.Services;
using CockyShop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace CockyShop.Controllers
{
    [Authorize(Policy = "RequireAdministratorRole")]
    public class UsersController : AppBaseController
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly IOrdersService _ordersService;

        public UsersController(SignInManager<AppUser> signInManager, IMapper mapper,
            IConfiguration configuration, IUserService userService, IOrdersService ordersService)
        {
            _signInManager = signInManager;
            _mapper = mapper;
            _configuration = configuration;
            _userService = userService;
            _ordersService = ordersService;
        }
        
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<LoggedUserDto>> Login([FromBody] LoggedUserRequest request)
        {
            var user = await _userService.FindUserByEmailAsync(request.Email);

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
                JwtToken = token,
                Roles = await _userService.GetAllUserRolesAsync(user)
            });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<AppUserDto>> Register([FromBody] RegisterUserRequest request)
        {
            await _userService.ValidateDuplicatedUserAsync(request);

            var user = new AppUser()
            {
                UserName = request.UserName,
                Email = request.Email
            };

            await _userService.CreateUserAsync(user, request.Password);

            await _userService.AddRoleToUserAsync(user, "CUSTOMER");

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
            return Ok(_mapper.Map<List<AppUserDto>>(await _userService.GetAllUsers()));
        }

        [HttpGet("email")]
        public async Task<ActionResult<AppUserDto>> GetUserByEmail([FromQuery] string email)
        {
            var user = await _userService.FindUserByEmailAsync(email);

            return Ok(new AppUserDto()
            {
                Email = user.Email,
                UserName = user.UserName,
                Orders = await _ordersService.GetAllOrdersByUserIdAsync(user.Id)
            });
        }
        
        [HttpPost("role")]
        public async Task<ActionResult<AppUserDto>> UpdateUserRole([FromBody] UserRoleRequest request)
        {
            var user = await _userService.FindUserByEmailAsync(request.Email);

            var role = await _userService.FindRoleByNameAsync(request.Role);
            
            await _userService.AddRoleToUserAsync(user, role.Name);
            
            return Ok(new LoggedUserDto()
            {
                User = new AppUserDto()
                {
                    Email = user.Email,
                    UserName = user.UserName,
                    Orders = null
                },
                Roles = await _userService.GetAllUserRolesAsync(user)

            });
        }

        [HttpDelete]
        public async Task<ActionResult<AppUserDto>> DeleteUser([FromQuery] string email)
        {
            var user = await _userService.FindUserByEmailAsync(email);

            await _userService.DeleteUserAsync(user);
            
            return NoContent();
        }
        
        
        private async Task<string> GenerateJwtToken(string email, AppUser user)
        {
           
            var userRoles = await _userService.GetAllUserRolesAsync(user);

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