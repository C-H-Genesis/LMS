
using CourseModel;
using UsersModel;

namespace TeacherModel
{
    public class Teacher
{
    public Guid TeacherId { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public required string TeacherName { get; set; } 
    public string? Department { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }

    // Navigation Property
    public User? User { get; set; }
    public required ICollection<Course> Courses { get; set; } 
}

}