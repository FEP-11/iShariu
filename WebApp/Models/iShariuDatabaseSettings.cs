namespace WebApp.Models
{
    public class iShariuDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string UsersCollectionName { get; set; } = null!;

        public string CoursesCollectionName { get; set; } = null!;
    }
}