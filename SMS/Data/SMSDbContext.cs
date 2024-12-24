using Microsoft.EntityFrameworkCore;
using AdminModel;
using FinanceModel;
using RoleModel;
using StudentModel;
using PermissionModel;
using RolePermissionModel;
using UsersModel;
using TeacherModel;

namespace ApplicationDbContext
{

    public class SMSDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Admin> Admin { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teacher { get; set; }
        public DbSet<Finance> Finance { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }


        public SMSDbContext(DbContextOptions<SMSDbContext> options) 
        : base(options)
    {
    }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

            // Configure RolePermission composite key
        modelBuilder.Entity<RolePermission>()
            .HasKey(rp => new { rp.RoleId, rp.PermissionId });

        // Configure many-to-many relationship between Role and Permission
        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rp => rp.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Permission)
            .WithMany(p => p.RolePermissions)
            .HasForeignKey(rp => rp.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);

         modelBuilder.Entity<Role>()
        .HasIndex(r => r.RoleName)
        .IsUnique();

        // Configure the relationship between User and Role
        modelBuilder.Entity<User>()
            .HasOne(u => u.Roles)
            .WithMany() // Assuming Role has a collection of Users
            .HasPrincipalKey(r => r.RoleName) // Use Role.Name as the principal key
            .HasForeignKey(u => u.Role)   // Use User.Role as the foreign key
            .OnDelete(DeleteBehavior.Restrict); // Optional: restrict deletion of related roles

    }
    
    }

}