using AutoMapper;
using ecommerce_api.DTO;
using ecommerce_api.Models;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using X.PagedList;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ecommerce_api.Repostitories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly EcomerceDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAccountRepository _accountRepository;

        public OrderRepository(EcomerceDbContext context, IMapper mapper, IAccountRepository accountRepository)
        {
            _context = context;
            _mapper = mapper;
            _accountRepository = accountRepository;
        }

        public async Task<Order> CancelOrder(string userName, int id)
        {
            var user = await _accountRepository.GetCurrentUserAsync(userName);
            var order=await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(o => o.Product)
                .Include(o => o.User)
                .Include(o => o.Payment)
                .Include(o => o.OrderStatus)
                .Include(o => o.Voucher)
                .FirstOrDefaultAsync(o=>o.OrderId == id&&o.UserId==user.Id);
            if(order != null)
            {
                order.OrderStatusId = 6;
                await _context.SaveChangesAsync();
            }
            return order;
        }

        public async Task<IEnumerable<Order>> FilterOrders(string? userName, int? statusId, int? shopId)
        {
            var query = _context.Orders
                .Include(p => p.OrderStatus)
                .Include(p => p.Payment)
                .Include(p => p.Voucher)
                .Include(p => p.User)
                .Include(p => p.OrderDetails)
                .ThenInclude(o => o.Product)
                .AsQueryable();
            if (!string.IsNullOrEmpty(userName))
            {
                query = query.Where(p => p.User.UserName==userName);
            }
            
            if (shopId.HasValue)
            {
                query = query.Where(p => p.OrderDetails.ElementAt(0).Product.ShopId == shopId );
            }
            if (statusId.HasValue)
            {
                query = query.Where(p => p.OrderStatusId ==statusId);
            }
           

            var orders = await query.OrderBy(p => p.OrderId).ToListAsync();
            return orders;
        }

        public async Task<Order?> GetOrderDetailByUser(string userName, int orderId)
        {
            var user = await _accountRepository.GetCurrentUserAsync(userName);
            return await _context.Orders
                .Where(o => o.UserId == user.Id)
                .Where(o => o.OrderId == orderId)
                .Include(o => o.OrderDetails)
                .ThenInclude(o => o.Product)
                .Include(o => o.User)
                .Include(o => o.Payment)
                .Include(o => o.OrderStatus)
                .Include(o => o.Voucher)
                .FirstOrDefaultAsync();
        }
        public async Task<Order?> GetOrderDetailByShop(int shopId, int orderId)
        {
           
            return await _context.Orders
                .Where(p => p.OrderDetails.ElementAt(0).Product.ShopId == shopId)
                .Where(o => o.OrderId == orderId)
                .Include(o => o.OrderDetails)
                .ThenInclude(o => o.Product)
                .Include(o => o.User)
                .Include(o => o.Payment)
                .Include(o => o.OrderStatus)
                .Include(o => o.Voucher)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Order>> GetUserOrder(string userName)
        {
            var user= await _accountRepository.GetCurrentUserAsync(userName);
            return await _context.Orders
                .Where(o=>o.UserId==user.Id)
                .Include(o=>o.OrderDetails)
                .ThenInclude(o=>o.Product)
                .Include(o=>o.User)
                .Include(o=>o.Payment)
                .Include(o=>o.OrderStatus)
                .Include(o=>o.Voucher)
                .Include(o=>o.User)
                .ToListAsync();
        }

        public async Task<Order> UpdateStatus(int shopId, int id)
        {
            var order = await GetOrderDetailByShop(shopId, id);
            if(order == null)
            {
                return order;
            }
            if (order.OrderStatusId < 5 || order.OrderStatusId == 9 || order.OrderStatusId == 10)
            {
                order.OrderStatusId++;
                await _context.SaveChangesAsync();

            }
            return order;
        }

        public async Task<Order> GiveBackOrder(string userName, int id)
        {
            var order = await GetOrderDetailByUser(userName, id);
            if (order == null)
            {
                return order;
            }
            order.OrderStatusId = 9;
            await _context.SaveChangesAsync();
            return order;
        }   
    }
}
