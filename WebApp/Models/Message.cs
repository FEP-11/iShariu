using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using WebApp.Interfaces;

namespace WebApp.Models
{
    public class Message : IIdentifiable
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        public string SkibidiMan { get; set; }
        public string Content { get; set; }
    }
}