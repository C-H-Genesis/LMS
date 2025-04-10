using UsersModel;
using CourseModel;

namespace GradeModel
{
        public class Grades
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public Guid CourseId { get; set; }
        public Course? Course { get; set; }
        public double Score { get; set; }
        public required string Remarks { get; set; }
        public DateTime GradedAt { get; set; }
    }

}