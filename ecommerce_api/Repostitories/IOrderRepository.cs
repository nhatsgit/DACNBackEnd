using ecommerce_api.DTO;
using ecommerce_api.Models;

namespace ecommerce_api.Repostitories
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetUserOrder(string userName);
        Task<Order> GetOrderDetail(string userName,int orderId);
    }
}
