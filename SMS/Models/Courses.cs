
using EnrollmentModel;
using UsersModel;

namespace CourseModel
{
    public class Course
    {
        public required Guid Id {get; set;}
        public required string CourseCode {get ; set;}
        public required string CourseName {get; set;}
        public Guid UserId { get; set;}
        
        public User? User { get; set; }
        public required ICollection<Enrollment> Enrollments { get; set; }  = new List<Enrollment>();

        
    }

}