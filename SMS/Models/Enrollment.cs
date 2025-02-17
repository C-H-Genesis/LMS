using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CourseModel;
using UsersModel;

namespace EnrollmentModel
{
    public class Enrollment
    {
        public int Id { get; set; }
        public required Guid CourseId { get; set; }
        
        public Guid UserId { get; set; }
        public required bool Status{ get; set; }
        public DateTime EnrollmentDate { get; set; }

         
        public Course? Courses { get; set; }
        public User? User { get; set; }

    } 
}


