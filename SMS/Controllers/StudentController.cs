using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApplicationDbContext;
using UsersModel;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using UpdateProfile;
using RegisterNewCourse;
using EnrollmentModel;
using CourseModel; 

namespace StudentController
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Student")]  // Authorize based on role stored in JWT
    public class StudentController : ControllerBase
    {
        private readonly SMSDbContext _context;

        public StudentController(SMSDbContext context)
        {
            _context = context;
        }

        // Get current student's profile information
       [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.Claims
                .FirstOrDefault(c => c.Type == "UserId")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated.");
            }

            // Fetch both user and student profile information from the database
            var student = await _context.Students
                .Include(s => s.User)  // Include related user data
                .FirstOrDefaultAsync(s => s.UserId.ToString() == userId);

            if (student == null)
            { 
                return NotFound("User not found.");
            }

            // Create response object with the required user and student information

            var userInfo = new
            {
                FullName = student.FullName,
                Username = student.User.Username,
                RegNumber= student.RegNumber,
                Email = student.Email,
                PhoneNumber = student.PhoneNumber
            };

            return Ok(userInfo);
        }


        // Update student's profile
       [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto updateUserDto)
        {
            var userId = User.Claims
                .FirstOrDefault(c => c.Type == "UserId")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated.");
            }

            var student = await _context.Students
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.UserId.ToString() == userId);

            if (student == null)
            {
                return NotFound("User not found.");
            }

            // Update user properties if the corresponding values are provided
           if (!string.IsNullOrEmpty(updateUserDto.UserName))  // Check for non-empty string
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                student.User.Username = updateUserDto.UserName;  // Update UserName in Users table
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }

            if (!string.IsNullOrEmpty(updateUserDto.FullName))  // Check for non-empty string
            {
                student.FullName = updateUserDto.FullName;  // Update FullName in Students table
            }

            if (!string.IsNullOrEmpty(updateUserDto.Email))  // Check for non-empty string
            {
                student.Email = updateUserDto.Email;  // Update Email in Students table
            }

            if (!string.IsNullOrEmpty(updateUserDto.RegNumber))  // Check for non-empty string
            {
                student.RegNumber = updateUserDto.RegNumber;  // Update RegNumber in Students table
            }

            if (!string.IsNullOrEmpty(updateUserDto.PhoneNumber))  // Check for non-empty string
            {
                student.PhoneNumber = updateUserDto.PhoneNumber;  // Update PhoneNumber in Students table
            }


            // Save changes
#pragma warning disable CS8604 // Possible null reference argument.
            _context.Users.Update(student.User);  // Update Users table
#pragma warning restore CS8604 // Possible null reference argument.
            _context.Students.Update(student);    // Update Students table
            await _context.SaveChangesAsync();

            var updatedUserInfo = new
            {
                student.User.Username,
                student.FullName,
                student.RegNumber,
                student.PhoneNumber,
                student.Email
            };

            return Ok(updatedUserInfo);
        }


        // Register for a course using Course Code
       [HttpPost("registerCourse")]
        public async Task<IActionResult> RegisterCourse([FromBody] RegisterCourseDto registerCourseDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.Claims
                .FirstOrDefault(c => c.Type == "UserId")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated.");
            }

            var userGuid = Guid.Parse(userId);

            var course = await _context.Courses
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CourseCode == registerCourseDto.CourseCode);

            if (course == null) return NotFound("Course not found.");

            var existingEnrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.UserId == userGuid && e.CourseId == course.Id);

            if (existingEnrollment != null)
                return BadRequest("You are already registered for this course.");

            var enrollment = new Enrollment
            {
                UserId = userGuid,
                CourseId = course.Id,
                Status = registerCourseDto.Status,
                EnrollmentDate = DateTime.UtcNow,
                
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            return Ok(new { Message = $"Successfully registered for the course: {course.CourseName}" });
        }

        // Get the student's registered courses
       [HttpGet("registered-courses")]
        public async Task<IActionResult> GetRegisteredCourses()
        {
            // Extract UserId from Claims
            var userId = User.Claims
                .FirstOrDefault(c => c.Type == "UserId")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated.");
            }

            var userGuid = Guid.Parse(userId);

            // Fetch the enrollments for the current student with course details

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var enrollments = await _context.Enrollments
            .Include(e => e.Courses) // Include the related Course data
            .Where(e => e.UserId == userGuid) // Filter by UserId (Student's enrolled courses)
            .Select(e => new
            {
                Course = new
                {
                    e.Courses.CourseCode, // Course code
                    e.Courses.CourseName  // Course name
                },
                e.Status,             // Enrollment status
                e.EnrollmentDate      // Date of enrollment
            })
            .ToListAsync();
#pragma warning restore CS8602 // Dereference of a possibly null reference.




            // If no enrollments found, return an empty array or a message
            if (enrollments == null || !enrollments.Any())
            {
                return NotFound("No courses found for the current user.");
            }

            // Return the list of registered courses
            return Ok(enrollments);
        }

    }
}
