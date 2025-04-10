using RolePermissionModel;
using UsersModel;
using UserRoles;

namespace RoleModel
{
    public class Role
{
    public Guid RoleId { get; set; }
    public required string RoleName { get; set; }

    // Navigation property for many-to-many relationship
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    public required ICollection<User> Users { get; set; } = new List<User>();
    
}

}