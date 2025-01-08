using TeacherModel;
using EnrollmentModel;

namespace CourseModel
{
    public class Course
    {
        public required Guid Id {get; set;}
        public required string CourseCode {get ; set;}
        public required string CourseName {get; set;}
        public string? TeacherName { get; set;}
        
        public Teacher? Teacher { get; set; }
        public required ICollection<Enrollment> Enrollments { get; set; }  = new List<Enrollment>();

        
    }

}