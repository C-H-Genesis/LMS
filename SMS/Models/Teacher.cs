
using UsersModel;

namespace TeacherModel
{
    public class Teacher
{
    public Guid TeacherId { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Department { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }

    // Navigation Property
    public User? User { get; set; }
}

}