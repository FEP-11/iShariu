using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebApp.Models
{
    public class Users
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name must be filled")]
        [MinLength(4, ErrorMessage = "Name must be at least 4 characters")]
        [MaxLength(20, ErrorMessage = "Name must be no more than 20 characters")]
        public string Username { get; set; }
        public string? Email { get; set; }
        [Required(ErrorMessage = "Password must be filled")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        public string Password { get; set; }
        [NotMapped]
        [JsonIgnore]
        [Required(ErrorMessage = "Confirm Password must be filled")]
        [Compare("Password", ErrorMessage = "Password don't match")]
        public string ConfirmPassword { get; set; }
        public string? Status { get; set; }
    }
}
