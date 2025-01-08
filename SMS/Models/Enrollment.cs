using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CourseModel;
using StudentModel;

namespace EnrollmentModel
{
    public class Enrollment
    {
        public int Id { get; set; }
        public required Guid CourseId { get; set; }
         [ForeignKey("Student")]
        public Guid UserId { get; set; }
        public required bool Status{ get; set; }
        public DateTime EnrollmentDate { get; set; }

        
        public Course? Courses { get; set; }
        public Student? Student { get; set;}

    } 
}


