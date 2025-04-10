using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ApplicationDbContext;
using CourseModel;
using EnrollmentModel;
using StudentModel;
using SubmissionDTO;
using SubmissionModel;
using GradeModel;
using AssignmentModel;
using CourseMaterial;
using GradeDTO;
using AssignmentDTO;
using FileUploadDto;


namespace TeacherControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Teacher")] // Ensures the user is authenticated
    public class TeachersController : ControllerBase
    {
        private readonly SMSDbContext _context;

        public TeachersController(SMSDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        } 
         [HttpGet("GetCourses")]
        public async Task<IActionResult> GetCourses()
        {
            // Fetch the userId from the JWT token (assumes userId is stored in token as 'userId')
            var teacherName = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
#pragma warning disable CS8604 // Possible null reference argument.
            var userGuid = Guid.Parse(teacherName);
#pragma warning restore CS8604 // Possible null reference argument. 


            if (teacherName == null)
            {
                return Unauthorized(new { message = "User ID not found in token" });
            }

            var teacherCourses = await _context.Courses
                .Where(c => c.UserId == userGuid) // Use the userId to fetch courses
                .ToListAsync();

            if (!teacherCourses.Any())
            {
                return NotFound(new { message = "No courses found for this teacher" });
            }

            return Ok(teacherCourses);
        }

        // GET: api/teachers/students/{courseCode}
       [HttpGet("students/{courseCode}")]
        public async Task<IActionResult> GetStudentsByCourse(string courseCode)
        {

           var course = await _context.Courses
            .Include(c => c.Enrollments)
                .ThenInclude(e => e.User) // Reference Student
            .FirstOrDefaultAsync(c => c.CourseCode == courseCode);


            if (course == null)
            {
                return NotFound(new { message = "Course not found" });
            }


            var students = course.Enrollments
                .Where(e => e.User is Student) // Ensure only students are included
                .Select(e => new
                {
                    RegNumber = (e.User as Student)?.RegNumber, // Safe cast to Student
                    e.User?.FullName
                })
                .ToList();

            return Ok(students);

        }

            //               Assignments                //

                     //  Upload The File First//

            [HttpPost("uploadAssignmentFile")]
            [Consumes("multipart/form-data")]
            public async Task<IActionResult> UploadAssignmentFile([FromForm] UploadAssignmentDto model)
            {
                if (model.File == null || model.File.Length == 0)
                {
                    return BadRequest("File is required.");
                }

                // Define the directory where files will be uploaded
                string uploadDirectory = Path.Combine("D:", "SMS&LMS", "sms", "Uploads");

                // Check if the directory exists, if not, create it
                if (!Directory.Exists(uploadDirectory))
                {
                    Directory.CreateDirectory(uploadDirectory);
                }

                // Combine the directory path with the file name
                var filePath = Path.Combine(uploadDirectory, model.File.FileName);

                // Save the file to the defined path
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.File.CopyToAsync(stream);
                }

                // Return the file URL for the frontend to use
                return Ok(new { FileUrl = $"/Uploads/{model.File.FileName}" });
            }



            [HttpPost("createNewAssignment")]
            public async Task<IActionResult> CreateAssignment([FromBody] AssignmentDto assignmentDto)
        {
                if (string.IsNullOrWhiteSpace(assignmentDto.Title) ||
                    string.IsNullOrWhiteSpace(assignmentDto.Description) ||
                    assignmentDto.CourseId == Guid.Empty)
                {
                    return BadRequest("Missing required fields: Title, Description, or CourseId.");
                }

                // Create the assignment
                var assignment = new Assignments
                {
                    Title = assignmentDto.Title,
                    Description = assignmentDto.Description,
                    WrittenAssignment = assignmentDto.WrittenAssignment,
                    CreatedAt = DateTime.UtcNow,
                    DueDate = assignmentDto.DueDate,
                    FileUrl = assignmentDto.FileUrl ?? "", // Store file URL if uploaded
                    CourseId = assignmentDto.CourseId
                };

                _context.Assignment.Add(assignment);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Assignment posted successfully" });
        }


            //             SUBMISSIONS               //

            [HttpGet("Submissions", Name = "GetAllSubmissionsRoute")]
            public async Task<IActionResult> GetAllSubmissions()
            {
                    var submissions = await _context.Submission.Include(s => s.Assignment)
                        .Include(s => s.User)
                        .Include(s => s.Course)
                        .ToListAsync();
                    
                    return Ok(submissions);
            }

            [HttpGet("Submissions/{Id}")]
            public async Task<IActionResult> GetSubmissionById(Guid id)
            {
                    var submission = await _context.Submission.Include(s => s.Assignment)
                        .Include(s => s.User)
                        .Include(s => s.Course)
                        .FirstOrDefaultAsync(s => s.Id == id);
                        if (submission == null) return NotFound();
                    return Ok(submission);
            }

            

            [HttpPut("UpdateSubmission/{id}")]
            public async Task<IActionResult> UpdateSubmission(Guid id, [FromBody] SubmissionDto submissionDto)
            {
                var submission = await _context.Submission.FindAsync(id);
                if (submission == null) return NotFound();

                submission.AssignmentId = submissionDto.AssignmentId;
                submission.UserId = submissionDto.UserId;
                submission.CourseId = submissionDto.CourseId;
                submission.FileUrl = submissionDto.FileUrl;
                submission.SubmittedAt = submissionDto.SubmittedAt;

                await _context.SaveChangesAsync();
                return NoContent();
            }

            [HttpDelete("DeleteSubmission/{id}")]
            public async Task<IActionResult> DeleteSubmission(Guid id)
            {
                var submission = await _context.Submission.FindAsync(id);
                if (submission == null) return NotFound();
                _context.Submission.Remove(submission);
                await _context.SaveChangesAsync();
                return NoContent();
            }
        // ✅ Grade a student submission
            [HttpPost("submissions/{id}/grade")]
            public async Task<IActionResult> GradeSubmissionAsync(Guid id, [FromBody] GradeDto gradeDto)
            {
                var submission = _context.Submission.FirstOrDefault(s => s.Id == id);
                if (submission == null) return NotFound("Submission not found");

                var grade = new Grades
                {
                    Id = Guid.NewGuid(),
                    UserId = submission.UserId, // Student who submitted
                    CourseId = submission.CourseId,
                    Score = gradeDto.Score,
                    Remarks = gradeDto.Remarks,
                    GradedAt = DateTime.UtcNow
                };

                _context.Grade.Add(grade);
                await  _context.SaveChangesAsync();

                return Ok(new { message = "Grade submitted successfully", grade });
            }
  }
}  


    