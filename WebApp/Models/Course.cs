using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using WebApp.Interfaces;

namespace WebApp.Models
{
    [BsonIgnoreExtraElements]
    public class Course : IIdentifiable
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string? CourseName { get; set; }
        public decimal CoursePrice { get; set; }
        public int Sales { get; set; }
        public decimal RevenueGenerated { get; set; }
        
        public List<string> UserIds { get; set; } = new List<string>();
        public string CreatorId { get; set; }
    }
}
