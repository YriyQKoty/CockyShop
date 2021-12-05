using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using CockyShop.Models.DTO;
using CockyShop.Models.Identity;
using CockyShop.Models.Requests;
using CockyShop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CockyShop.Controllers
{
    [Authorize(Policy = "RequireAdministratorRole")]
    public class OrdersController : AppBaseController
    {
        private readonly IOrdersService _ordersService;
        

        public OrdersController(IOrdersService ordersService)
        {
            _ordersService = ordersService;
        }
        
        [Authorize(Roles = "Customer,Admin")]
        [HttpGet("email")]
        public async Task<ActionResult<List<OrderDto>>> GetAllUserOrdersByEmail([FromQuery] string email)
        {
            if (!HttpContext.User.IsInRole("Admin"))
            {
                if (email != HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier))
                {
                    return Forbid();
                }
            }
            
            var orders = await _ordersService.GetAllOrdersByEmailAsync(email);

            return orders;
        }
        
        [HttpGet("userId")]
        public async Task<ActionResult<List<OrderDto>>> GetAllUserOrdersByUserId([FromQuery] string userId)
        {
            var orders = await _ordersService.GetAllOrdersByUserIdAsync(userId);

            return orders;
        }

        [Authorize(Roles = "Customer,Admin")]
        [HttpGet("{orderId}")]
        public async Task<ActionResult<OrderDto>> GetOrderById([FromQuery] string email,[FromRoute] int orderId)
        {
            if (!HttpContext.User.IsInRole("Admin"))
            {
                if (email != HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier))
                {
                    return Forbid();
                }
            }
            return await _ordersService.GetOrderByIdAsync(orderId);
        }
        
        [Authorize(Roles = "Customer,Admin")]
        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateUserOrder([FromBody] OrderRequest request)
        {
          
            if (request.UserEmail != HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return Forbid();
            }

            return await _ordersService.CreateOrderAsync(request);
        }
        
        [Authorize(Policy = "RequireAdministratorRole")]
        [HttpPut("User/OrderStatus/All")]
        public async Task<ActionResult<List<OrderDto>>> UpdateAllUserOrderStatuses([FromQuery] string userId,
            [FromBody] OrderStatusRequest request)
        {
            return await _ordersService.ChangeAllUserOrderStatusesAsync(userId,request);
        }
        
        [Authorize(Policy = "RequireAdministratorRole")]
        [HttpPut("User/OrderStatus/{orderId}")]
        public async Task<ActionResult<OrderDto>> UpdateUserOrderStatusById([FromQuery] string userId, 
            [FromRoute] int orderId,
            [FromBody] OrderStatusRequest request)
        {
            return await _ordersService.ChangeOrderStatusByIdAsync(userId,orderId,request);
        }
        
        [Authorize(Roles = "Customer,Admin")]
        [HttpPut("User/OrderStatus/Cancel/{orderId}")]
        public async Task<ActionResult<OrderDto>> UpdateOrderStatusToCancelled([FromQuery] string userId, 
            [FromRoute] int orderId)
        {
            return await _ordersService.CancelOrder(userId,orderId);
        }
        
      
    }
}