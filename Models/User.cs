using System.ComponentModel.DataAnnotations;

namespace NageshaJewellers.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        public string Phone { get; set; }

        [Required]
        public string Password { get; set; }
    }
}