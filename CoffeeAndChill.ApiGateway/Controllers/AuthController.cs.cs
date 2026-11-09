using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using CoffeeAndChill.ApiGateway.DTOs;
using CoffeeAndChill.ApiGateway.Models;
using CoffeeAndChill.ApiGateway.Services;

namespace CoffeeAndChill.ApiGateway.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserTableService _userService;
        private readonly IPasswordHasher<UserEntity> _passwordHasher;

        public AuthController(
            UserTableService userService,
            IPasswordHasher<UserEntity> passwordHasher)
        {
            _userService = userService;
            _passwordHasher = passwordHasher;
        }

        /// <summary>
        /// Registers a new Staff user.
        /// </summary>
        /// <remarks>
        /// Creates a user account and securely hashes the password.
        /// </remarks>
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
    }
}