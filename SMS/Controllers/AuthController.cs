using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Collections.Generic; // For List<T> 
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using System.Text;
using AdminModel;
using FinanceModel;
using RoleModel;
using StudentModel;
using UsersModel;
using TeacherModel;
using UserRoles;
using ApplicationDbContext;
using LoginDTO;
using RegisterDTO;
using CourseModel;
using System.Linq;


namespace AuthController
{
    [ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly SMSDbContext _context;

    public AuthController(SMSDbContext context)
    {
        _context = context;
    }

    // Registration Method
    [HttpPost("register")]
    public async Task <IActionResult> Register([FromBody] RegisterRequest request)
    {
        // Check if the username is already taken
        var existingUser = await _context.Users.AnyAsync(u => u.Username == request.Username);
        if (existingUser)
        {
            return BadRequest("Username already exists.");
        }

        var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == request.Role);
        if (role == null)
        {
            return BadRequest("Invalid role specified.");
        }

        // Hash the password
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // Create the user
        var user = new User
        {
            UserId = Guid.NewGuid(),
            Username = request.Username,
            PasswordHash = hashedPassword,
            Role = request.Role,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        // Add to specific role-based table
        switch (request.Role)
        {
            case "Student":
                var student = new Student
                {
                    UserId = user.UserId,
                    FullName = request.FullName,
                    EnrollmentDate = DateTime.UtcNow,
                    User = user
                    
                };
                _context.Students.Add(student);
                await _context.SaveChangesAsync();
                break;

            case "Admin":
                var admin = new Admin
                {
                    AdminId = Guid.NewGuid(),
                    UserId = user.UserId,
                    FullName = request.FullName
                };
                _context.Admin.Add(admin);
                break;

            case "Finance":
                var finance = new Finance
                {
                    FinanceId = Guid.NewGuid(),
                    UserId = user.UserId,
                    FullName = request.FullName
                };
                _context.Finance.Add(finance);
                break;

             case "Teacher":
                var teacher = new Teacher
                {
                    TeacherId = Guid.NewGuid(),
                    UserId = user.UserId,
                    TeacherName = request.FullName,
                    Courses = new List<Course>()
                };
                _context.Teacher.Add(teacher);
                break;

            default:
                return BadRequest("Invalid role specified.");
        }

        var userRole = new UserRole
        {
            UserId = user.UserId,
            RoleId = role.RoleId
        };
          _context.UserRoles.Add(userRole);
          
          var save = await _context.SaveChangesAsync();

          if (save > 0)
          {
            return Ok(new { message = "Registration successful." });

          }
          else
          {
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Registration failed.",
                Detail = "An unexpected error occurred while saving user details.",
                
            });
          } 
          
    } 

    // Login Method (Already implemented)
    [HttpPost("login")]
    public async Task <IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == request.Username);

        if (user == null)
        {
            return Unauthorized("Invalid username or password.");
        }

        // 2. Verify the input password matches the hashed password in the database
        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!isPasswordValid)
        {
            return Unauthorized("Invalid username or password.");
        }
        
        var token = GenerateToken(user);

        return Ok(new { Token = token});
    }

    // Token generation method (unchanged)
    private string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("UserId", user.UserId.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ThisisYourSecretKeyHereof32bytes"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "YourIssuer",
            audience: "YourAudience",
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

}
