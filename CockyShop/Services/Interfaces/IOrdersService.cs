using System.Collections.Generic;
using System.Threading.Tasks;
using CockyShop.Models.App;
using CockyShop.Models.DTO;
using CockyShop.Models.Requests;

namespace CockyShop.Services.Interfaces
{
    public interface IOrdersService
    {
        Task<List<OrderDto>> GetAllOrdersByEmailAsync(string email);
        
        Task<List<OrderDto>> GetAllOrdersByUserIdAsync(string userId);

        Task<OrderDto> GetOrderByIdAsync(int id);

        Task<OrderDto> CreateOrderAsync(OrderRequest request);

        Task<OrderDto> ChangeOrderStatusByIdAsync(int id, OrderStatusRequest request);

        Task<List<OrderDto>> ChangeAllUserOrderStatusesAsync(string userId, OrderStatusRequest request);
    }
}