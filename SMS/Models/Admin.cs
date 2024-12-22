

using Users;

namespace AdminModel
{
    public class Admin
{
    public Guid AdminId { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }

    // Navigation Property
    public User? User { get; set; }
}

}