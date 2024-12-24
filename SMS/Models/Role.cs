using RolePermissionModel;
using UsersModel;


namespace RoleModel
{
    public class Role
{
    public Guid RoleId { get; set; }
    public required string RoleName { get; set; }

    // Navigation property for many-to-many relationship
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    public User? User { get; set; }
}

}