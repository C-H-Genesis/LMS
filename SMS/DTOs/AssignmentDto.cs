
namespace AssignmentDTO
{
        public class AssignmentDto
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required DateTime DueDate { get; set; }
        public Guid CourseId { get; set; }
    }
}