using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DetectiveMysteryGamePlatform.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            if (model == null)
            {
                return BadRequest(new { message = "Login request data is required" });
            }

            if (string.IsNullOrWhiteSpace(model.Email))
            {
                return BadRequest(new { message = "Email is required" });
            }

            if (string.IsNullOrWhiteSpace(model.Password))
            {
                return BadRequest(new { message = "Password is required" });
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            var token = GenerateJwtToken(user);
            if (string.IsNullOrEmpty(token))
            {
                return StatusCode(500, new { message = "Failed to generate authentication token" });
            }

            return Ok(new { token });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { message = "Logged out successfully" });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest model)
        {
            if (model == null)
            {
                return BadRequest(new { message = "Reset password request data is required" });
            }

            if (string.IsNullOrWhiteSpace(model.Email))
            {
                return BadRequest(new { message = "Email is required" });
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return Ok(new { message = "If your email is registered, you will receive a password reset link" });
            }

            // In a real implementation, generate a reset token and send email
            // For the MVP, we'll just return a success message
            return Ok(new { message = "If your email is registered, you will receive a password reset link" });
        }

        private string GenerateJwtToken(IdentityUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var secretKey = _configuration["JwtSettings:SecretKey"];
            var issuer = _configuration["JwtSettings:Issuer"];
            var audience = _configuration["JwtSettings:Audience"];
            var expiryMinutes = _configuration["JwtSettings:ExpiryInMinutes"];

            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("JWT secret key is not configured");
            }

            if (string.IsNullOrEmpty(issuer))
            {
                throw new InvalidOperationException("JWT issuer is not configured");
            }

            if (string.IsNullOrEmpty(audience))
            {
                throw new InvalidOperationException("JWT audience is not configured");
            }

            if (string.IsNullOrEmpty(expiryMinutes))
            {
                throw new InvalidOperationException("JWT expiry time is not configured");
            }

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(Convert.ToDouble(expiryMinutes));

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class ResetPasswordRequest
    {
        public string Email { get; set; }
    }
} 