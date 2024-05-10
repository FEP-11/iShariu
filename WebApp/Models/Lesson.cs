using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApp.Models
{
    [BsonIgnoreExtraElements]
    public class Lesson
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string VideoUrl { get; set; }
        public string TextContent { get; set; }
    }
}