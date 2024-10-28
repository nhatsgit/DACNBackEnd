using AutoMapper;
using ecommerce_api.DTO;
using ecommerce_api.Models;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

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
        public async Task<Order?> GetOrderDetail(string userName, int orderId)
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
    }
}
