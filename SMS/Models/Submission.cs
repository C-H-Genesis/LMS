using UsersModel;
using CourseModel;
using AssignmentModel;

namespace SubmissionModel
{
        public class Submission
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid AssignmentId { get; set; }  // Foreign key to Assignment
        public Assignments? Assignment { get; set; }
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public Guid CourseId { get; set; }
        public Course? Course { get; set; }
        public required string FileUrl { get; set; }
        public string? WrittenSubmission { get; set; }
        public DateTime SubmittedAt { get; set; }
    }

}