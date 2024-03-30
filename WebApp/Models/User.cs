using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebApp.Models
{
    public class User
    {
        public string Username { get; set; }
        public string? Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } = "user";
    }
}
