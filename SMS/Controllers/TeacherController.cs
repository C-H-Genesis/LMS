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
            _context = context;
        } 

        // GET: api/teachers/courses
        [HttpGet("courses")]
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

        //       Assignments    //

        [HttpPost("createNewAssignment")]
    public async Task<IActionResult> CreateAssignment([FromBody] Assignments assignment)
    {
        if (assignment == null)
            return BadRequest("Invalid data");

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

            

            [HttpPut("{id}")]
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

            [HttpDelete("{id}")]
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


    