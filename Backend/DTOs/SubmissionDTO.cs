
namespace SubmissionDTO
{
     public class SubmissionDto
    {
        public Guid AssignmentId { get; set; }
        public Guid UserId { get; set; }
        public Guid CourseId { get; set; }
        public required string FileUrl { get; set; }
        public string? WrittenSubmission { get; set; }
        public DateTime SubmittedAt { get; set; }
    }

}