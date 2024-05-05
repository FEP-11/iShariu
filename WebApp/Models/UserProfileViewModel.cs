using System.Collections.Generic;
using WebApp.Models;

public class UserProfileViewModel
{
    public User User { get; set; }
    public List<Course> CreatedCourses { get; set; }
    public List<Course> EnrolledCourses { get; set; }
}