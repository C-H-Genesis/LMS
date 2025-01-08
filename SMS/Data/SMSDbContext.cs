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


namespace ApplicationDbContext
{

    public class SMSDbContext : DbContext
    {
        public required DbSet<User> Users { get; set; }
        public required DbSet<Admin> Admin { get; set; }
        public required DbSet<Student> Students { get; set; }
        public required DbSet<Teacher> Teacher { get; set; }
        public required DbSet<Finance> Finance { get; set; }
        public required DbSet<Role> Roles { get; set; }
        public required DbSet<Permission> Permissions { get; set; }
        public required DbSet<RolePermission> RolePermissions { get; set; }
        public required DbSet<Course> Courses { get; set; }
        public required DbSet<Enrollment> Enrollments { get; set; }
        public required DbSet<UserRole> UserRoles { get; set; }


        public SMSDbContext(DbContextOptions<SMSDbContext> options) 
        : base(options)
    {
    }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

         modelBuilder.Entity<Role>()
        .HasIndex(r => r.RoleName) 
        .IsUnique();

        // Configure the relationship between User and Role
       modelBuilder.Entity<User>()
        .HasOne<Role>() // Specify the related entity is Role
        .WithMany(r => r.Users) // No navigation property in Role pointing to Users
        .HasForeignKey(u => u.Role) // User.Role stores the RoleName
        .HasPrincipalKey(r => r.RoleName) // Use Role.RoleName as the principal key
        .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of Role if associated Users exist


        modelBuilder.Entity<Student>()
        .ToTable("Students") // Map to the correct table name
        .HasKey(s => s.UserId); // Define the primary key

        modelBuilder.Entity<Student>()
        .HasOne(s => s.User)
        .WithOne()
        .HasForeignKey<Student>(s => s.UserId);


            // Configure RolePermission composite key
        modelBuilder.Entity<RolePermission>()
            .HasKey(rp => new { rp.RoleId, rp.PermissionId });

        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });   

       modelBuilder.Entity<Course>()
        .HasOne(c => c.Teacher)
        .WithMany(t => t.Courses)
        .HasForeignKey(c => c.TeacherName) // Explicitly map the foreign key
        .HasPrincipalKey(t => t.TeacherName);          
            
        modelBuilder.Entity<Teacher>()
        .HasIndex(t => t.TeacherName) 
        .IsUnique();    

        modelBuilder.Entity<Enrollment>()
        .ToTable("Enrollments")
        .HasKey(e => e.Id);

        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Courses) // Reference to Course
            .WithMany(c => c.Enrollments) // Bidirectional relationship
            .HasForeignKey(e => e.CourseId) // Foreign key in Enrollment
            .OnDelete(DeleteBehavior.Cascade);

       modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(s => s.UserId); // Primary key

            entity.HasOne(s => s.User) // Navigation property to Users
                .WithOne() // Assuming a one-to-one relationship
                .HasForeignKey<Student>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Adjust cascade as needed
        });


        
        modelBuilder.Entity<Course>()
        .HasMany(c => c.Enrollments)
        .WithOne(e => e.Courses)
        .HasForeignKey(e => e.CourseId)
        .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Student>()
        .HasOne(s => s.User)
        .WithOne(u => u.Student)
        .HasForeignKey<Student>(s => s.UserId)
        .OnDelete(DeleteBehavior.Cascade);

 
    }
    
    } 

}