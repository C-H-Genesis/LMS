

using Users;

namespace FinanceModel
{
    public class Finance
{
    public Guid FinanceId { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }

    // Navigation Property
    public User? User { get; set; }
}

}