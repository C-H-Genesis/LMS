
using Users;

namespace StudentModel
{
    public class Student
{
    public Guid StudentId { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string RegNumber { get; set; } = string.Empty;
    public DateTime EnrollmentDate { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }

    // Navigation Property
    public User? User { get; set; }
}

}