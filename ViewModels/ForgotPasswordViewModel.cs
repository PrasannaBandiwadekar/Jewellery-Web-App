using System.ComponentModel.DataAnnotations;

namespace NageshaJewellers.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}