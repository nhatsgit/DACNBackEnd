using AutoMapper;
using ecommerce_api.DTO;
using ecommerce_api.Models;
using ecommerce_api.Repostitories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using X.PagedList;

namespace ecommerce_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository orderRepository;
        private readonly IMapper _mapper;

        public OrdersController(IOrderRepository _orderRepository, IMapper mapper) {
            orderRepository = _orderRepository;
            _mapper = mapper;
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userName == null)
            {
                return Unauthorized();
            }
            var orders = await orderRepository.GetUserOrder(userName);
            
            return Ok(new PagedListDTO<OrderDTO>(_mapper.Map<IEnumerable<OrderDTO>>(orders).ToPagedList(pageNumber, pageSize)));
        }
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Get(int id)
        {
            var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userName == null)
            {
                return Unauthorized();
            }
            var order = await orderRepository.GetOrderDetailByUser(userName,id);
            if(order == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<OrderDTO>(order));
        }
        [HttpPost("Cancel")]
        [Authorize]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userName == null)
            {
                return Unauthorized();
            }
            var order = await orderRepository.CancelOrder(userName,id);
            if(order == null)
            {
                return NotFound();
            }
            return Ok("Canceled");
        }
        [HttpPost("GiveBack")]
        [Authorize]

        public async Task<IActionResult> GiveBackOrder(int id)
        {
            var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userName == null)
            {
                return Unauthorized();
            }
            

            var order = await orderRepository.GiveBackOrder(userName, id);
            if (order == null)
            {
                return NotFound();
            }
            return Ok("Successs");
        }
    }
}
