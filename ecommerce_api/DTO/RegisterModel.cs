using System.ComponentModel.DataAnnotations;

namespace ecommerce_api.Models
{
    public class RegisterModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Address { get; set; }
        [Required, EmailAddress]
        public string? Email { get; set; }
        [Required, Phone]
        public string? PhoneNumber { get; set; }

    }
}
