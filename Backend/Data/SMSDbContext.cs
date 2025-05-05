using Microsoft.EntityFrameworkCore;
using AdminModel;
using FinanceModel;
using RoleModel;
using StudentModel;
using PermissionModel;
using RolePermissionModel;
using UsersModel;
using TeacherModel;
using EnrollmentModel;
using CourseModel;
using UserRoles;
using AssignmentModel;
using GradeModel;
using CourseMaterial;
using SubmissionModel;

namespace ApplicationDbContext
{
    public class SMSDbContext : DbContext
    {
        public required DbSet<User> Users { get; set; }
        public required DbSet<Role> Roles { get; set; }
        public required DbSet<Permission> Permissions { get; set; }
        public required DbSet<RolePermission> RolePermissions { get; set; }
        public required DbSet<Course> Courses { get; set; }
        public required DbSet<Enrollment> Enrollments { get; set; }
        public required DbSet<UserRole> UserRoles { get; set; }
        public required DbSet<Assignments> Assignment { get; set; }
        public required DbSet<Grades> Grade { get; set; }
        public required DbSet<Materials> Material { get; set; }
         public required DbSet<Submission> Submission { get; set; }
        

        public SMSDbContext(DbContextOptions<SMSDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure User inheritance using TPH
            modelBuilder.Entity<User>()
                .HasDiscriminator<string>("UserType")
                .HasValue<User>("User")
                .HasValue<Student>("Student")
                .HasValue<Teacher>("Teacher")
                .HasValue<Admin>("Admin")
                .HasValue<Finance>("Finance");

            modelBuilder.Entity<User>()
                .ToTable("Users"); // All entities in the inheritance hierarchy share this table

            modelBuilder.Entity<Admin>()
                .ToTable("Users"); // Inherited from User, same table as the base class

            modelBuilder.Entity<Finance>()
                .ToTable("Users");    

            modelBuilder.Entity<User>()
                .HasMany(u => u.Roles)
                .WithMany(r => r.Users)
                .UsingEntity<UserRole>(
                    j => j
                        .HasOne(ur => ur.Role)
                        .WithMany(r => r.UserRoles)
                        .HasForeignKey(ur => ur.RoleId),
                    j => j
                        .HasOne(ur => ur.User)
                        .WithMany(u => u.UserRoles)
                        .HasForeignKey(ur => ur.UserId),
                    j =>
                    {
                        j.HasKey(ur => new { ur.UserId, ur.RoleId });
                        j.ToTable("UserRoles");
                    });


            // Configure unique RoleName in Roles table
            modelBuilder.Entity<Role>()
                .HasIndex(r => r.RoleName)
                .IsUnique();

            // RolePermission composite key
            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });

            // UserRole composite key
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);
    

            // Enrollment relationships
            modelBuilder.Entity<Enrollment>()
                .ToTable("Enrollments")
                .HasKey(e => e.Id);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.User)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Courses)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId);  

            modelBuilder.Entity<Assignments>()
            .HasOne(a => a.Teacher)
            .WithMany() // Or .WithMany(t => t.Assignments) if you have a collection in `User`
            .HasForeignKey(a => a.TeacherId);
            



            base.OnModelCreating(modelBuilder);
        }
    }
}
