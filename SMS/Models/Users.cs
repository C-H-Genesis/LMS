using AdminModel;
using FinanceModel;
using RoleModel;
using StudentModel;

namespace UsersModel{
    public class User
{
    public Guid UserId { get; set; } = Guid.NewGuid();
    public required string Username { get; set; } 
    public required string PasswordHash { get; set; }
    public required string Role { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public Role? Roles { get; set; }
    public Student? Student { get; set; }
    public Admin? Admin { get; set; }
    public Finance? Finance { get; set; }
}

}