﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using WebApp.Interfaces;

namespace WebApp.Models
{
    [BsonIgnoreExtraElements]
    public class User : IIdentifiable
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } = UserRole.User;
        public string Location { get; set; } = "UA";
        public DateTime JoinDate { get; set; } = DateTime.UtcNow; 
        public List<string> CreatedCourses { get; set; } = new List<string>();
        public List<string> EnrolledCourses { get; set; } = new List<string>();
        public int Sales { get; set; } = 0;
        public decimal RevenueGenerated { get; set; } = 0m;
        public string PhoneNumber { get; set; }
        public bool HasMFAEnabled { get; set; }
        public bool AllowAccessToAgeRestrictedContent { get; set; }
        public bool UseDataToImproveIShariu { get; set; }
        public string ProfileColor { get; set; } = "#808080";
    }
}