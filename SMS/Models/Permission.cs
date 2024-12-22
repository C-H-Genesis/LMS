using RolePermissionModel;


namespace PermissionModel
{
    public class Permission
{
    public int PermissionId { get; set; }
    public required string PermissionName { get; set; }

    // Navigation property for many-to-many relationship
    public ICollection<RolePermission>? RolePermissions { get; set; }
}


}