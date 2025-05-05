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
using StudentModel;
using SubmissionModel;
using SubmissionDTO;

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

    // Fetch the user (which could be a Student, Teacher, etc.)
    var user = await _context.Users
        .FirstOrDefaultAsync(u => u.UserId.ToString() == userId);

    if (user == null)
    {
        return NotFound("User not found.");
    }

    // Check if user is a student and access RegNumber
    if (user is Student student)
    {
        var userInfo = new
        {
            FullName = user.FullName,
            Username = user.Username,
            RegNumber = student.RegNumber, // Access RegNumber from the Student class
            Email = user.Email,
            PhoneNumber = user.PhoneNumber
        };

        return Ok(userInfo);
    }

    return BadRequest("User is not a student.");
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

    // Fetch the user (which could be a Student, Teacher, etc.)
    var user = await _context.Users
        .FirstOrDefaultAsync(u => u.UserId.ToString() == userId);

    if (user == null)
    {
        return NotFound("User not found.");
    }

    // Check if the user is a student and fetch the Student-specific details
    if (user is Student student)
    {
        if (!string.IsNullOrEmpty(updateUserDto.UserName))  // Check for non-empty string
        {
            user.Username = updateUserDto.UserName;  // Update Username in Users table
        }

        if (!string.IsNullOrEmpty(updateUserDto.FullName))  // Check for non-empty string
        {
            user.FullName = updateUserDto.FullName;  // Update FullName in Users table
        }

        if (!string.IsNullOrEmpty(updateUserDto.Email))  // Check for non-empty string
        {
            user.Email = updateUserDto.Email;  // Update Email in Users table
        }

        if (!string.IsNullOrEmpty(updateUserDto.PhoneNumber))  // Check for non-empty string
        {
            user.PhoneNumber = updateUserDto.PhoneNumber;  // Update PhoneNumber in Users table
        }

        if (!string.IsNullOrEmpty(updateUserDto.RegNumber))  // Check for non-empty string
        {
            student.RegNumber = updateUserDto.RegNumber;  // Update RegNumber in Student (same table)
        }

        // Save changes
        _context.Users.Update(user);    // Update Users table
        await _context.SaveChangesAsync();

        var updatedUserInfo = new
        {
            user.Username,
            user.FullName,
            student.RegNumber, // Access RegNumber from the Student class
            user.PhoneNumber,
            user.Email
        };

        return Ok(updatedUserInfo);
    }

    return BadRequest("User is not a student.");
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

            // Fetch the student from the database
            var student = await _context.Users.FirstOrDefaultAsync(s => s.UserId == userGuid);
            if (student == null)
            {
                return NotFound("Student not found.");
            }

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
            var userId = User.Claims
                .FirstOrDefault(c => c.Type == "UserId")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated.");
            }

            var userGuid = Guid.Parse(userId);

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var enrollments = await _context.Enrollments
                .Include(e => e.Courses)  // Include the related Course data
                .Where(e => e.UserId == userGuid)  // Filter by UserId (Student's enrolled courses)
                .Select(e => new
                {
                    e.Courses.CourseCode,
                    e.Courses.CourseName,
                    e.Status,
                    e.EnrollmentDate
                })
                .ToListAsync();
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            if (enrollments == null || !enrollments.Any())
            {
                return NotFound("No courses found for the current user.");
            }

            return Ok(enrollments);
        }

        //    Assignment     //

        [HttpGet("Assignments")]
        public async Task<IActionResult> GetAssignmentsForEnrolledCourses()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated.");
            }

            var userGuid = Guid.Parse(userId);

            // Get the list of courses the user is enrolled in
            var enrolledCourses = await _context.Enrollments
                .Where(e => e.UserId == userGuid)
                .Select(e => e.CourseId)
                .ToListAsync();

            if (!enrolledCourses.Any())
            {
                return NotFound("You are not enrolled in any courses.");
            }

            // Get assignments for all enrolled courses
            var assignments = await _context.Assignment
                .Where(a => enrolledCourses.Contains(a.CourseId))
                .ToListAsync();

            if (assignments == null || !assignments.Any())
            {
                return NotFound("No assignments found for the courses you are enrolled in.");
            }

            return Ok(assignments);
        }

            [HttpPost("PostSubmission")]
        public async Task<IActionResult> PostSubmission([FromBody] SubmissionDto submissionDto)
        {
            if (string.IsNullOrWhiteSpace(submissionDto.FileUrl) && string.IsNullOrWhiteSpace(submissionDto.WrittenSubmission))
            {
                return BadRequest("You must provide either a file URL or a written response.");
            }

            var submission = new Submission
            {
                AssignmentId = submissionDto.AssignmentId,
                UserId = submissionDto.UserId,
                CourseId = submissionDto.CourseId,
                FileUrl = submissionDto.FileUrl,
                WrittenSubmission = submissionDto.WrittenSubmission,
                SubmittedAt = submissionDto.SubmittedAt
            };

            _context.Submission.Add(submission);
            await _context.SaveChangesAsync();
            return CreatedAtRoute("GetAllSubmissionsRoute", new { id = submission.Id }, submission);
        }
    }
}
