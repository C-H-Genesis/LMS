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
using UserRoles;
using FinanceModel;
using AdminModel;
using TeacherModel;
using RegisterDTO;
using StudentModel;
using UpdateProfile;
using UpdateUserInfoDto;


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

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            Console.WriteLine($"API received request for user ID: {id}");
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpGet("user/{role}")]
        public async Task<IActionResult> GetUsersByRoleAsync(string role)
        {
                    // Query the Users table for users with the specified role
                    var users = await _context.Users
                .Where(u => u.UserType == role)
                .ToListAsync();

                    // Check if any users were found
                    if (users == null ) 
                return NotFound($"No users found with the role: {role}");

            // Return the list of users
            return Ok(users);
        }

       [HttpPut("profile/{userId}")]
        public async Task<IActionResult> UpdateProfile(Guid userId, [FromBody] UpdateUserInfo updateUserDto)
        {
            // Check if the authenticated user is an admin
            var role = User.Claims.FirstOrDefault(c => c.Type == "Role")?.Value;
            if (role != "Admin")
            {
                return Unauthorized("Only admins can update user profiles.");
            }

            // Find the user by userId
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return NotFound("User not found."); 
            }

            // Update fields if new values are provided
            if (!string.IsNullOrEmpty(updateUserDto.UserName))  
                user.Username = updateUserDto.UserName;

            if (!string.IsNullOrEmpty(updateUserDto.FullName))  
                user.FullName = updateUserDto.FullName;

            if (!string.IsNullOrEmpty(updateUserDto.Email))  
                user.Email = updateUserDto.Email;

            if (!string.IsNullOrEmpty(updateUserDto.PhoneNumber))  
                user.PhoneNumber = updateUserDto.PhoneNumber;

            // Update role (only if admin is changing it)
           if (!string.IsNullOrEmpty(updateUserDto.Role))
            {
                var roleEntity = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == updateUserDto.Role);
                if (roleEntity == null)
                {
                    return BadRequest("Invalid role selected.");
                }
                
                user.Role = roleEntity; // Assign the role entity object
            }



            // Save changes
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                user.Username,
                user.FullName,
                user.Email,
                user.PhoneNumber,
                user.Role
            });
        }


        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser([FromBody] RegisterRequest request)
        {
            // Check if the username already exists
            var existingUser = await _context.Users.AnyAsync(u => u.Username == request.Username);
            if (existingUser)
            {
                return BadRequest(new { Message = "Username already exists." });
            }

            // Fetch the role
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == request.Role);
            if (role == null)
            {
                return BadRequest(new { Message = "Invalid role specified." });
            }

            // Hash the password
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // Create the user entity based on the role
            User user = request.Role switch
            {
                "Student" => new Student
                {
                    UserId = Guid.NewGuid(),
                    FullName = request.FullName,
                    Username = request.Username,
                    PasswordHash = hashedPassword,
                    RoleId = role.RoleId,
                    UserType = "Student",
                    EnrollmentDate = DateTime.UtcNow
                },
                "Admin" => new Admin
                {
                    UserId = Guid.NewGuid(),
                    FullName = request.FullName,
                    Username = request.Username,
                    PasswordHash = hashedPassword,
                    RoleId = role.RoleId,
                    UserType = "Admin",
                },
                "Finance" => new Finance
                {
                    UserId = Guid.NewGuid(),
                    FullName = request.FullName,
                    Username = request.Username,
                    PasswordHash = hashedPassword,
                    RoleId = role.RoleId,
                    UserType = "Finance",
                },
                "Teacher" => new Teacher
                {
                    UserId = Guid.NewGuid(),
                    FullName = request.FullName,
                    Username = request.Username,
                    PasswordHash = hashedPassword,
                    RoleId = role.RoleId,
                    UserType = "Teacher",
                },
                _ => throw new ArgumentException("Invalid role specified.")
            };

            if (user == null)
            {
                return BadRequest(new { Message = "Failed to create user entity." });
            }

            // Add user and user-role mapping
            _context.Users.Add(user);
            var userRole = new UserRole
            {
                UserId = user.UserId,
                RoleId = role.RoleId
            };
            await _context.UserRoles.AddAsync(userRole);

            // Save changes to the database
            var saveResult = await _context.SaveChangesAsync();
            if (saveResult > 0)
            {
                return Ok(new { Message = "User created successfully." });
            }

            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Failed to create user.",
                Detail = "An unexpected error occurred while saving user details."
            });
        }

        [HttpDelete("delete-user/{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            // Find the user by ID
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == Guid.Parse(userId));
            if (user == null)
            {
                return NotFound(new { Message = "User not found." });
            }

            // Remove the user and related UserRole entry
            var userRole = await _context.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == user.UserId);
            if (userRole != null)
            {
                _context.UserRoles.Remove(userRole);
            }
            _context.Users.Remove(user);

            // Save changes to the database
            var saveResult = await _context.SaveChangesAsync();
            if (saveResult > 0)
            {
                return Ok(new { Message = "User deleted successfully." });
            }

            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Failed to delete user.",
                Detail = "An unexpected error occurred while deleting the user."
            });
        }

        [HttpGet("courses")]
        public async Task<IActionResult> GetAllCoursesAsync()
        {
            var courses = await _context.Courses
                .Select(c => new 
                {
                    c.Id,
                    c.CourseCode,
                    c.CourseName,
                    TeacherName = _context.Users
                        .Where(u => u.UserId == c.UserId)  // Assuming TeacherId in Courses references UserId in Users
                        .Select(u => u.FullName) // Replace with actual column name
                        .FirstOrDefault()
                })
                .ToListAsync();
            return Ok(courses);
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
                return Conflict( new { message ="A course with this Course Code already exists."});
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.FullName == courseDto.TeacherName);
            if (user == null)
            {
                return NotFound(new { message ="User with the provided username does not exist."});
            }

            // Create new Course object
            var newCourse = new Course
            {
                Id = Guid.NewGuid(),
                CourseCode = courseDto.CourseCode,
                CourseName = courseDto.CourseName, 
                UserId = user.UserId,
                Enrollments = new List<Enrollment>()
                
            };
            

            // Add to the database
            _context.Courses.Add(newCourse);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Course added successfully." });
        }

        [HttpDelete("delete-course/{Id}")]
        public async Task<IActionResult> DeleteCourse(string Id)
        {

            // Remove the user and related UserRole entry
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == Guid.Parse(Id));
            if (course != null)
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
                 return Ok(new { Message = "Course Deletion successfully." });
            }

            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Failed to delete Course.",
                Detail = "An unexpected error occurred while deleting the Course."
            });
        }

    }

}