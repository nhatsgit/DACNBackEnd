using ecommerce_api.DTO;
using ecommerce_api.Models;
using System.Reflection.Metadata;

namespace ecommerce_api.Repostitories
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetUserOrder(string userName);
        Task<IEnumerable<Order>> FilterOrders(string? userName,int? statusId,int? shopId);
        Task<Order> GetOrderDetailByUser(string userName,int orderId);
        Task<Order> GetOrderDetailByShop(int shopId, int orderId);
        Task<Order> CancelOrder(string userName, int id);
        Task<Order> UpdateStatus(int shopId, int id);
        Task<Order> GiveBackOrder(string userName, int id);

    }
}
