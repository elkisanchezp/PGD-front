using System.ComponentModel.DataAnnotations;

namespace TesisMarco.Models
{
    public class LoginModel
    {
        [Required]
        public string Usuario { get; set; }

        [Required]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
