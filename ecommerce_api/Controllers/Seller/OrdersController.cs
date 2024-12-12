using AutoMapper;
using ecommerce_api.DTO;
using ecommerce_api.Repostitories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using X.PagedList;

namespace ecommerce_api.Controllers.Seller
{
    [Route("api/seller/[controller]")]
    [ApiController]
    [Authorize(Roles = "Developer,ShopOwner,ShopStaff")]

    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository orderRepository;
        private readonly IMapper _mapper;
        private readonly IAccountRepository _accountRepository;


        public OrdersController(IOrderRepository _orderRepository, IMapper mapper, IAccountRepository accountRepository)
        {
            orderRepository = _orderRepository;
            _mapper = mapper;
            _accountRepository = accountRepository;

        }
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5, [FromQuery] DateTime? date = null)
        {
            var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userName == null)
            {
                return Unauthorized();
            }
            var user = await _accountRepository.GetCurrentUserAsync(userName);
            var orders = await orderRepository.FilterOrders(null, null, user.ShopId,date);

            return Ok(new PagedListDTO<OrderDTO>(_mapper.Map<IEnumerable<OrderDTO>>(orders).ToPagedList(pageNumber, pageSize)));
        }
        [HttpGet("NotConfirm")]
        public async Task<IActionResult> GetOrdersNotConfirm([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userName == null)
            {
                return Unauthorized();
            }
            var user = await _accountRepository.GetCurrentUserAsync(userName);
            var orders = await orderRepository.FilterOrders(null,1,user.ShopId,null);

            return Ok(new PagedListDTO<OrderDTO>(_mapper.Map<IEnumerable<OrderDTO>>(orders).ToPagedList(pageNumber, pageSize)));
        }
        [HttpGet("Delivered")]
        public async Task<IActionResult> GetOrdersDelivered([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userName == null)
            {
                return Unauthorized();
            }
            var user = await _accountRepository.GetCurrentUserAsync(userName);
            var orders = await orderRepository.FilterOrders(null,6,user.ShopId,null);

            return Ok(new PagedListDTO<OrderDTO>(_mapper.Map<IEnumerable<OrderDTO>>(orders).ToPagedList(pageNumber, pageSize)));
        }
        [HttpGet("RequestReturn")]
        public async Task<IActionResult> GetOrdersRequestReturn([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userName == null)
            {
                return Unauthorized();
            }
            var user = await _accountRepository.GetCurrentUserAsync(userName);
            var orders = await orderRepository.FilterOrders(null,9,user.ShopId, null);

            return Ok(new PagedListDTO<OrderDTO>(_mapper.Map<IEnumerable<OrderDTO>>(orders).ToPagedList(pageNumber, pageSize)));
        }
        [HttpGet("Returned")]
        public async Task<IActionResult> GetOrdersReturned([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userName == null)
            {
                return Unauthorized();
            }
            var user = await _accountRepository.GetCurrentUserAsync(userName);
            var orders = await orderRepository.FilterOrders(null, 11, user.ShopId, null);

            return Ok(new PagedListDTO<OrderDTO>(_mapper.Map<IEnumerable<OrderDTO>>(orders).ToPagedList(pageNumber, pageSize)));
        }[HttpGet("Canceled")]
        public async Task<IActionResult> GetOrdersCanceled([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userName == null)
            {
                return Unauthorized();
            }
            var user = await _accountRepository.GetCurrentUserAsync(userName);
            var orders = await orderRepository.FilterOrders(null, 6, user.ShopId,null);

            return Ok(new PagedListDTO<OrderDTO>(_mapper.Map<IEnumerable<OrderDTO>>(orders).ToPagedList(pageNumber, pageSize)));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userName == null)
            {
                return Unauthorized();
            }
            var user = await _accountRepository.GetCurrentUserAsync(userName);
            if (user?.ShopId == null)
            {
                return BadRequest("Người dùng không thuộc shop nào.");
            }
            var order = await orderRepository.GetOrderDetailByShop(user.ShopId??0, id);
            if (order == null)
            {
                return BadRequest();
            }
            return Ok(_mapper.Map<OrderDTO>(order));
        }
        
        [HttpPost("UpdateStatus")]
        public async Task<IActionResult> UpdateOrder(int orderId)
        {
            var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userName == null)
            {
                return Unauthorized();
            }
            var user = await _accountRepository.GetCurrentUserAsync(userName);
            if (user?.ShopId == null)
            {
                return BadRequest("Người dùng không thuộc shop nào.");
            }
            var order = await orderRepository.UpdateStatus(user.ShopId ?? 0, orderId);
            
            if (order == null)
            {
                return NotFound();
            }
            return Ok("Success");
            
        }
    }
}
