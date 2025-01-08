using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using CourseModel; 
using ApplicationDbContext;
using UsersModel;
using AddNewCourse;
using EnrollmentModel;


namespace AdminController
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin")]

    public class AdminController : ControllerBase
    {
        private readonly SMSDbContext _context;

        public AdminController(SMSDbContext context)
        {
            _context = context;
        }

        [HttpPost("Add-New-Course")]
        public async Task<IActionResult> CreateNewCourse( [FromBody] AddCourseDto courseDto)
        {
            if (string.IsNullOrWhiteSpace(courseDto.CourseCode) || string.IsNullOrWhiteSpace(courseDto.CourseName) || string.IsNullOrWhiteSpace(courseDto.TeacherName))
            {
                return BadRequest("Course Code and Course Name are required.");
            }

            // Check if course already exists
            var existingCourse = await _context.Courses
                .FirstOrDefaultAsync(c => c.CourseCode == courseDto.CourseCode);
            if (existingCourse != null)
            {
                return Conflict("A course with this Course Code already exists.");
            }

            // Create new Course object
            var newCourse = new Course
            {
                Id = Guid.NewGuid(),
                CourseCode = courseDto.CourseCode,
                CourseName = courseDto.CourseName,
                TeacherName = courseDto.TeacherName,
                Enrollments = new List<Enrollment>()
                
            };
            

            // Add to the database
            _context.Courses.Add(newCourse);
            await _context.SaveChangesAsync();

            return Ok("Course added successfully.");
        }
    }

}