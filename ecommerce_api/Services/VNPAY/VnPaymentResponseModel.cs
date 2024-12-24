using ecommerce_api.DTO;
using ecommerce_api.Models;
using Microsoft.Identity.Client;

namespace ecommerce_api.Services.VNPAY
{
    public class VnPaymentResponseModel
    {
        public bool Success { get; set; }
        public string PaymentMethod { get; set; }
        public string OrderDescription { get; set; }
        public string OrderId { get; set; }
        public string PaymentId { get; set; }
        public string TransactionId { get; set; }
        public string Token { get; set; }
        public string VnPayResponseCode { get; set; }
    }

    public class VnPaymentRequestModel
    {
        public int OrderId { get; set; }
        public double Amount { get; set; }
        public DateTime CreatedDate { get; set; }
        public CheckOutDTO CheckOutDTO { get; set; }
        public int shoppingCartId { get; set; }
    }
    public class OrderSessionModel
    {
        public Order Order { get; set; }
        public int shopId { get; set; }
    }
}
