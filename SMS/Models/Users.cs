using AdminModel;
using FinanceModel;
using RoleModel;
using StudentModel;
using TeacherModel;
using UserRoles;
using EnrollmentModel;

namespace UsersModel{
    public class User
{
    public Guid UserId { get; set; } = Guid.NewGuid();
    public required string Username { get; set; } 
    public required string PasswordHash { get; set; }
    public required string Role { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public Student? Student { get; set; }
    public ICollection<Admin>? Admins { get; set; }
    public ICollection<Finance>? Finances { get; set; }
    public ICollection<Teacher>? Teachers { get; set; }
    public ICollection<Enrollment>? Enrollments { get; set; }
}

}


