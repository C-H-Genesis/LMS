using RoleModel;
using EnrollmentModel;
using UserRoles;

namespace UsersModel{
    public class User
{
    public Guid UserId { get; set; } = Guid.NewGuid();
    public required string FullName { get; set; }
    public required string Username { get; set; } 
    public required string PasswordHash { get; set; }
    public Guid RoleId { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? UserType { get; set; }
    public string? PasswordResetToken { get; set; }
    public DateTime? ResetTokenExpiry { get; set; }

    

    // Navigation Properties

    public ICollection<Role> Roles { get; set; } = new List<Role>();
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public ICollection<Enrollment>? Enrollments { get; set; }  = new List<Enrollment>();
    
}

}


