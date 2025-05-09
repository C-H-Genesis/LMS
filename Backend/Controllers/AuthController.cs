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
using EmailAuth;
using System.Security.Cryptography;
using ResetPasswordDTO;
using ForgotPasswordDTO;


namespace AuthController
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly SMSDbContext _context;
        private readonly EmailService _emailService;

        public AuthController(SMSDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
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
            string generatedPassword = PasswordGenerator.GeneratePassword(12);
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(generatedPassword);

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
                    EnrollmentDate = DateTime.UtcNow,
                    Email = request.Email
                },
                "Admin" => new Admin
                {
                    UserId = Guid.NewGuid(),
                    FullName = request.FullName,
                    Username = request.Username,
                    PasswordHash = hashedPassword,
                    RoleId = role.RoleId,
                    Email = request.Email,
                    UserType = "Admin"
                },
                "Finance" => new Finance
                {
                    UserId = Guid.NewGuid(),
                    FullName = request.FullName,
                    Username = request.Username,
                    PasswordHash = hashedPassword,
                    RoleId = role.RoleId,
                    Email = request.Email,
                    UserType = "Finance"
                },
                "Teacher" => new Teacher
                {
                    UserId = Guid.NewGuid(),
                    FullName = request.FullName,
                    Username = request.Username,
                    PasswordHash = hashedPassword,
                    RoleId = role.RoleId,
                    Email = request.Email,
                    UserType = "Teacher"
                },
                _ => throw new ArgumentException("Invalid role specified.")
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
            await _context.UserRoles.AddAsync(userRole);

            // Save changes
            var save = await _context.SaveChangesAsync();

            if (save > 0)
            {
                // ✅ Send welcome email
                try
                {
                    string subject = "Welcome to SMS-LMS Platform";
                    string body = $"Hi {request.FullName},\n\n" +
                                $"You have successfully registered as a {request.Role}.\n" +
                                $"Your username is: {request.Username}\n" +
                                $"Your temporary password is: {generatedPassword}\n\n" +
                                "Please change your password after logging in.\n\nThank you for joining our platform.";
                                        

                    await _emailService.SendEmailAsync(request.Email, subject, body); // Assuming Username is the email
                }
                catch (Exception ex)
                {
                    // Log or handle email errors if needed
                    Console.WriteLine($"Email send failed: {ex.Message}");
                }

                return Ok(new { message = "Registration successful and email sent." });
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

        //            Reset Password             //

                [HttpPost("reset-password")]
                public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
                {
                    var trimmedToken = request.Token?.Trim();

                    var user = await _context.Users.FirstOrDefaultAsync(u =>
                        u.Email == request.Email &&
                        u.PasswordResetToken == trimmedToken &&
                        u.ResetTokenExpiry > DateTime.UtcNow);
   

                    if (user == null)
                        return BadRequest("Invalid or expired reset token.");

                    // Hash and set the new password
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

                    // Clear token fields
                    user.PasswordResetToken = null;
                    user.ResetTokenExpiry = null;

                    await _context.SaveChangesAsync();

                    return Ok(new { message = "Your password has been reset successfully."});
                }


        //               Forgot Password                 //
                    [HttpPost("forgot-password")]
            public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
            {
                if (string.IsNullOrWhiteSpace(request.Email))
                    return BadRequest("Email is required.");

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (user == null)
                    return NotFound("No user associated with this email.");

                // Generate reset token
                var resetToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
                user.PasswordResetToken = resetToken;
                user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1); // Token valid for 1 hour

                await _context.SaveChangesAsync();

                // Send reset link
                var encodedToken = Uri.EscapeDataString(resetToken);
                var resetLink = $"http://localhost:4200/reset-password?token={encodedToken}&email={request.Email}";
                var subject = "Password Reset Request";
                var body = $"Hi {user.FullName},\n\nClick the link below to reset your password:\n{resetLink}\n\nIf you didn’t request this, ignore this email.";

                await _emailService.SendEmailAsync(user.Email, subject, body);

                return Ok(new { message = "Password reset link has been sent to your email."});
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
                claims.Add(new Claim(ClaimTypes.Role, role.RoleName));
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


            public class PasswordGenerator
        {
            public static string GeneratePassword(int length = 12)
            {
                const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()-_=+";
                
                if (length <= 0)
                    throw new ArgumentException("Password length must be greater than 0.");

                var chars = new char[length];
                using (var rng = RandomNumberGenerator.Create())
                {
                    byte[] uintBuffer = new byte[sizeof(uint)];

                    for (int i = 0; i < length; i++)
                    {
                        rng.GetBytes(uintBuffer);
                        uint num = BitConverter.ToUInt32(uintBuffer, 0);
                        chars[i] = valid[(int)(num % (uint)valid.Length)];
                    }
                }

                return new string(chars);
            }
        }
}


