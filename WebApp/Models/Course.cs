﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace WebApp.Models
{
        public class Course
        {
            [BsonId]
            [BsonRepresentation(BsonType.ObjectId)]
            public string Id { get; set; }
            public string? CourseName { get; set; }
            public decimal CoursePrice { get; set; }
            public int Sales { get; set; }
            public decimal RevenueGenerated { get; set; }
        }
    }