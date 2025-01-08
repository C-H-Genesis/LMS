
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EnrollmentModel;
using UsersModel;

namespace StudentModel
{
    public class Student
{
    [Key]
    [ForeignKey("User")]
    public Guid UserId { get; set; }
    public required User User{ get; set; }
    public required string FullName { get; set; }
    public string? RegNumber { get; set; } = string.Empty;
    public DateTime EnrollmentDate { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }

    public ICollection<Enrollment>? Enrollments { get; set; }

}

}