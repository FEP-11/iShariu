using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using WebApp.Interfaces;

namespace WebApp.Models
{
    [BsonIgnoreExtraElements]
    public class Lesson : IIdentifiable
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string VideoUrl { get; set; }
        public string TextContent { get; set; }
    }
}