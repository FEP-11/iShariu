using System.Linq.Expressions;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using WebApp.Controllers;
using WebApp.Interfaces;
using Microsoft.Extensions.Logging;

namespace WebApp.Services
{
    public class MongoDBService<T> where T : IIdentifiable
    {
        private readonly IMongoCollection<T> _collection;
        private readonly IMongoDatabase _database;

        public MongoDBService(IOptions<iShariuDatabaseSettings> settings, ILogger<MongoDBService<T>> logger)
        {
            MongoClient client = new MongoClient(settings.Value.ConnectionString);
            IMongoDatabase database = client.GetDatabase(settings.Value.DatabaseName);

            string collectionName = typeof(T) switch
            {
                var t when t == typeof(User) => settings.Value.UsersCollectionName,
                var t when t == typeof(Course) => settings.Value.CoursesCollectionName,
                var t when t == typeof(Lesson) => settings.Value.LessonsCollectionName,
                var t when t == typeof(Message) => settings.Value.MessagesCollectionName,
            };
            _collection = database.GetCollection<T>(collectionName);
            _database = client.GetDatabase(settings.Value.DatabaseName);

            // Create collections if they don't exist
            CreateCollectionIfNotExists(settings.Value.UsersCollectionName);
            CreateCollectionIfNotExists(settings.Value.CoursesCollectionName);
            CreateCollectionIfNotExists(settings.Value.LessonsCollectionName);
            CreateCollectionIfNotExists(settings.Value.MessagesCollectionName);

            InitializeOnDbCreation();
        }

        public async Task<List<T>> GetAsync() => await _collection.Find(new BsonDocument()).ToListAsync();

        public async Task<T> GetAsync(string id)
        {
            FilterDefinition<T> filter = Builders<T>.Filter.Eq("Id", id);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<List<T>> GetAsync(FilterDefinition<T> filter) =>
            await _collection.Find(filter).Limit(10).ToListAsync();

        public async Task PostAsync(T item)
        {
            if (string.IsNullOrEmpty(item.Id))
                item.Id = ObjectId.GenerateNewId().ToString();

            await _collection.InsertOneAsync(item);
        }

        public async Task PutAsync(T item)
        {
            FilterDefinition<T> filter = Builders<T>.Filter.Eq("Id", item.Id);
            await _collection.ReplaceOneAsync(filter, item);
        }

        public async Task DeleteAsync(string id)
        {
            FilterDefinition<T> filter = Builders<T>.Filter.Eq("Id", id);
            await _collection.DeleteOneAsync(filter);
        }

        public async Task<List<T>> GetBestSellingCoursesAsync()
        {
            FilterDefinition<T> filter = Builders<T>.Filter.Empty;

            if (typeof(T) == typeof(Course))
            {
                Expression<Func<T, object>> sortExpression = item => ((Course)(object)item).RevenueGenerated;
                var sort = Builders<T>.Sort.Descending(sortExpression);
                return await _collection.Find(filter).Limit(3).Sort(sort).ToListAsync();
            }
            else
            {
                return await _collection.Find(filter).ToListAsync();
            }
        }

        [NonAction]
        private void CreateCollectionIfNotExists(string collectionName)
        {
            var filter = new BsonDocument("name", collectionName);
            var options = new ListCollectionsOptions { Filter = filter };
            bool exists = _database.ListCollections(options).Any();

            if (!exists) _database.CreateCollection(collectionName);
        }

        [NonAction]
        private async Task InitializeOnDbCreation()
        {
            var filter = new BsonDocument("name", "User");
            var options = new ListCollectionsOptions { Filter = filter };

            if (!_database.ListCollections(options).Any()) return;

            var userCollection = _database.GetCollection<User>("User");
            var courseCollection = _database.GetCollection<Course>("Course");
            var lessonCollection = _database.GetCollection<Lesson>("Lesson");

            List<User> users = new List<User>
            {
                new User
                {
                    Username = "user",
                    Email = "user1@example.com",
                    Role = UserRole.User,
                    Password = UserRole.User,
                    Location = "UA",
                    JoinDate = DateTime.UtcNow,
                    CreatedCourses = new List<string>(),
                    EnrolledCourses = new List<string>(),
                    Sales = 0,
                    RevenueGenerated = 0m,
                    PhoneNumber = "1234567890",
                    HasMFAEnabled = false,
                    AllowAccessToAgeRestrictedContent = false,
                    UseDataToImproveIShariu = false,
                    ProfileColor = "#808080"
                },
                new User
                {
                    Username = "creator",
                    Email = "creator1@example.com",
                    Role = UserRole.Creator,
                    Password = UserRole.Creator,
                    Location = "UA",
                    JoinDate = DateTime.UtcNow,
                    CreatedCourses = new List<string>(),
                    EnrolledCourses = new List<string>(),
                    Sales = 0,
                    RevenueGenerated = 0m,
                    PhoneNumber = "1234567890",
                    HasMFAEnabled = false,
                    AllowAccessToAgeRestrictedContent = false,
                    UseDataToImproveIShariu = false,
                    ProfileColor = "#808080"
                },
                new User
                {
                    Username = "admin",
                    Email = "admin1@example.com",
                    Role = UserRole.Admin,
                    Password = UserRole.Admin,
                    Location = "UA",
                    JoinDate = DateTime.UtcNow,
                    CreatedCourses = new List<string>(),
                    EnrolledCourses = new List<string>(),
                    Sales = 0,
                    RevenueGenerated = 0m,
                    PhoneNumber = "1234567890",
                    HasMFAEnabled = false,
                    AllowAccessToAgeRestrictedContent = false,
                    UseDataToImproveIShariu = false,
                    ProfileColor = "#808080"
                }
            };

            List<Course> courses = new List<Course>
            {
                new Course
                {
                    CourseName = "Course 1",
                    CourseDescription = "Description for Course 1",
                    CourseCategory = "Category 1",
                    CourseDifficulty = "Beginner",
                    CourseLanguage = "English",
                    CourseRating = "5",
                    CourseHours = "10",
                    CourseAccess = "Public",
                    CoursePrice = 100m,
                    Sales = 0,
                    RevenueGenerated = 0m,
                    UserIds = new List<string>(),
                    CreatorId = "creator1",
                    LessonIds = new List<string>()
                },
                new Course
                {
                    CourseName = "Course 2",
                    CourseDescription = "Description for Course 2",
                    CourseCategory = "Category 2",
                    CourseDifficulty = "Intermediate",
                    CourseLanguage = "English",
                    CourseRating = "4",
                    CourseHours = "20",
                    CourseAccess = "Public",
                    CoursePrice = 200m,
                    Sales = 0,
                    RevenueGenerated = 0m,
                    UserIds = new List<string>(),
                    CreatorId = "creator1",
                    LessonIds = new List<string>()
                }
            };

            List<Lesson> lessons = new List<Lesson>
            {
                new Lesson
                {
                    Name = "Mozart", VideoUrl = "https://www.youtube.com/watch?v=Ziai7G38giY", TextContent = "Content 1"
                },
                new Lesson
                {
                    Name = "Nemchinskiy", VideoUrl = "https://www.youtube.com/watch?v=y4AvvI025oc",
                    TextContent = "Content 2"
                },
                new Lesson
                {
                    Name = "Nemchinskiy 2", VideoUrl = "https://www.youtube.com/watch?v=jipybKSCvJk",
                    TextContent = "Content 3"
                },
                new Lesson
                {
                    Name = "Nemchinskiy 3", VideoUrl = "https://www.youtube.com/watch?v=u7fpOY29Gxc",
                    TextContent = "Content 4"
                },
                new Lesson
                {
                    Name = "C vs C++ vs C#", VideoUrl = "https://www.youtube.com/watch?v=1KPEeOBl_X8",
                    TextContent = "Content 5"
                },
                new Lesson
                {
                    Name = "Mewing day 100", VideoUrl = "https://www.youtube.com/watch?v=FJ-IFxUp5Cg",
                    TextContent = "Content 6"
                }
            };

            for (int i = 0; i < lessons.Count; i++)
            {
                await lessonCollection.InsertOneAsync(lessons[i]);
                courses[i / 3].LessonIds.Add(lessons[i].Id);
            }

            foreach (User user in users)
                await userCollection.InsertOneAsync(user);
            foreach (var course in courses)
                await courseCollection.InsertOneAsync(course);
            foreach (var lesson in lessons)
                await lessonCollection.InsertOneAsync(lesson);
        }
    }
}