using System.ComponentModel.DataAnnotations;

namespace FileUploadDto
{
    public class UploadAssignmentDto
{
    [Required]
    public required IFormFile File { get; set; }
}

}