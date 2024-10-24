using System.ComponentModel.DataAnnotations;

namespace ecommerce_api.Models
{
    public class LoginModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
       
    }
}
