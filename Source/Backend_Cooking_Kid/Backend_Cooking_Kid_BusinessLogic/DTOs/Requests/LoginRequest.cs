using System.ComponentModel.DataAnnotations;

namespace Backend_Cooking_Kid_BusinessLogic.DTOs.Requests
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "UserName is required")]
        public string UserName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
        [Required(ErrorMessage = "")]
        public string Unit { get; set; } = string.Empty;
    }

}
