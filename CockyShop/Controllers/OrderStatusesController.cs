using System.Linq;
using System.Threading.Tasks;
using CockyShop.Exceptions;
using CockyShop.Infrastucture;
using CockyShop.Models.DTO;
using CockyShop.Models.Enums;
using CockyShop.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CockyShop.Controllers
{
    [Authorize(Policy = "RequireAdministratorRole")]
    public class OrderStatusesController : AppBaseController
    {
        private AppDbContext _appDbContext;

        public OrderStatusesController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
   
        [HttpGet]
        public async Task<IActionResult> GetOrderStatuses()
        {
            var statuses =  await _appDbContext.OrderStatuses.Select(os => new OrderStatusDto()
            {
                Id = os.Id,
                StatusName = os.StatusName
            }).ToListAsync();

            return Ok(statuses);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrderStatus([FromBody] OrderStatusRequest request)
        {
            var status = await _appDbContext.OrderStatuses.SingleOrDefaultAsync(os => os.StatusName == request.Name);

            if (status != null)
            {
                throw new DomainException($"Order status with name '{request.Name}' already exists!");
            }

            var os = await _appDbContext.OrderStatuses.AddAsync(new OrderStatus() {StatusName = request.Name});
            await _appDbContext.SaveChangesAsync();

            return Ok(new OrderStatusDto()
            {
                Id = os.Entity.Id,
                StatusName = request.Name
            });
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderStatus([FromRoute]int id)
        {
            var status = await _appDbContext.OrderStatuses.SingleOrDefaultAsync(os => os.Id == id);

            if (status == null)
            {
                throw new DomainException($"Order status with such id '{id}' does not exist!");
            }

            _appDbContext.OrderStatuses.Remove(status);
            await _appDbContext.SaveChangesAsync();

            return NoContent();
        }
    }

 
}