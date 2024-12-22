using AdminModel;
using FinanceModel;
using RoleModel;
using StudentModel;

namespace UsersModel{
    public class User
{
    public Guid UserId { get; set; } = Guid.NewGuid();
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public Guid RoleId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? PhoneNumber { get; set; }

    // Navigation Properties
    public Role? Role { get; set; }
    public Student? Student { get; set; }
    public Admin? Admin { get; set; }
    public Finance? Finance { get; set; }
}

}