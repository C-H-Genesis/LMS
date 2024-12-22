

using RolePermissionModel;

namespace RoleModel
{
    public class Role
{
    public Guid RoleId { get; set; }
    public required string RoleName { get; set; }

    // Navigation property for many-to-many relationship
    public required ICollection<RolePermission> RolePermissions { get; set; }
}

}