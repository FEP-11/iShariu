using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace WebApp.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } = UserRole.User;
        
        public string Location { get; set; }
        public DateTime JoinDate { get; set; } = DateTime.UtcNow; 

        // These fields are only relevant for creators
        public List<Course>? Courses { get; set; } = new List<Course>();
        public int Sales { get; set; } = 0;
        public decimal RevenueGenerated { get; set; } = 0m;
        
        public List<string> CourseIds { get; set; } = new List<string>();
    }
}