
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using CoffeeAndChill.ApiGateway.DTOs;
using CoffeeAndChill.ApiGateway.Models;
using CoffeeAndChill.ApiGateway.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace CoffeeAndChill.ApiGateway.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserTableService _userService;
        private readonly IPasswordHasher<UserEntity> _passwordHasher;
        private readonly IConfiguration _config;

        public AuthController(
            UserTableService userService,
            IPasswordHasher<UserEntity> passwordHasher,
            IConfiguration config)
        {
            _userService = userService;
            _passwordHasher = passwordHasher;
            _config = config;
        }

        /// <summary>
        /// Registers a new Staff user.
        /// </summary>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register(
            [FromBody] RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Name, email and password are required.");
            }

            if (request.Password.Length < 8)
            {
                return BadRequest("Password must contain at least 8 characters.");
            }

            var existingUser = await _userService.GetUserByEmailAsync(request.Email);

            if (existingUser != null)
            {
                return Conflict("An account with this email already exists.");
            }

            var user = new UserEntity
            {
                Name = request.Name.Trim(),
                Email = request.Email.Trim().ToLowerInvariant(),
                Role = "Staff"
            };

            user.PasswordHash = _passwordHasher.HashPassword(
                user,
                request.Password
            );

            await _userService.CreateUserAsync(user);

            return StatusCode(201, new
            {
                message = "User registered successfully.",
                userId = user.UserId,
                name = user.Name,
                email = user.Email,
                role = user.Role
            });
        }

        /// <summary>
        /// Logs in and returns a signed JWT.
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login(
            [FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Email and password are required.");
            }

            var user = await _userService.GetUserByEmailAsync(request.Email);

            if (user == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            var result = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Password
            );

            if (result == PasswordVerificationResult.Failed)
            {
                return Unauthorized("Invalid email or password.");
            }

            int minutes = int.Parse(_config["Jwt:ExpiryMinutes"] ?? "60");
            var expires = DateTime.UtcNow.AddMinutes(minutes);

            return Ok(new
            {
                token = CreateToken(user, expires),
                expires,
                role = user.Role,
                name = user.Name
            });
        }

        /// <summary>
        /// Creates a signed JWT for the authenticated user.
        /// </summary>
        private string CreateToken(UserEntity user, DateTime expires)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
            );

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256
                )
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
