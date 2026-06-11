using System.ComponentModel.DataAnnotations;

namespace NageshaJewellers.ViewModels
{
    public class ResetPasswordViewModel
    {
        public string Email { get; set; }

        [Required]
        public string OTP { get; set; }

        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; }
    }
}