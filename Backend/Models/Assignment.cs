using CourseModel;
using UsersModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace AssignmentModel
{
        public class Assignments
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Title { get; set; } = string.Empty;
        public required string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public string FileUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public Guid CourseId { get; set; }
         public Course? Course { get; set; }
         [ForeignKey("TeacherId")]
        public Guid TeacherId { get; set; }
        public User? Teacher { get; set; }
        public string? WrittenAssignment { get; set;}
    }

}