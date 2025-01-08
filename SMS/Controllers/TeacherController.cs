using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ApplicationDbContext;
using CourseModel;
using EnrollmentModel;



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
            var teacherName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;


            if (teacherName == null)
            {
                return Unauthorized(new { message = "User ID not found in token" });
            }

            var teacherCourses = await _context.Courses
                .Where(c => c.TeacherName == teacherName) // Use the userId to fetch courses
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
                .ThenInclude(e => e.Student) // Reference Student
            .FirstOrDefaultAsync(c => c.CourseCode == courseCode);


            if (course == null)
            {
                return NotFound(new { message = "Course not found" });
            }


            var students = course.Enrollments.Select(e => new
                {
                    e.Student?.RegNumber,
                    e.Student?.FullName
                }).ToList();

            return Ok(students);
        }

    }
}
