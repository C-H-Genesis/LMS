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
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
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

            // Create the user with a UserType based on the role
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
                _  => throw new ArgumentException("Invalid role specified.")
            };

            if (user == null)
            {
                return BadRequest("Invalid role specified.");
            }

            // Add the user to the database
            _context.Users.Add(user);

            // Add UserRole entry
            var userRole = new UserRole
            {
                UserId = user.UserId,
                RoleId = role.RoleId
            };
           await  _context.UserRoles.AddAsync(userRole);

            // Save changes
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
                    Detail = "An unexpected error occurred while saving user details."
                });
            }
        }

        // Login Method
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Username and password are required.");
            }

            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            // Verify the input password matches the hashed password in the database
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                return Unauthorized("Invalid username or password.");
            }

           
                var token = GenerateToken(user);
                return Ok(new { token = token });

        }

        // Token generation method (unchanged)
            private string GenerateToken(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User object cannot be null.");
            }

            if (string.IsNullOrEmpty(user.Username))
            {
                throw new InvalidOperationException("User.Username cannot be null or empty.");
            }

            if (user.Roles == null || !user.Roles.Any())
            {
                throw new InvalidOperationException("User role information is not available.");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("UserId", user.UserId.ToString())
            };

            foreach (var role in user.Roles)
            {
                claims.Add(new Claim("Role", role.RoleName));
            }

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


