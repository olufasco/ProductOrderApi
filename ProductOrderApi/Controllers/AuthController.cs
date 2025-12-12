using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductOrderApi.Common;
using ProductOrderApi.Data;
using ProductOrderApi.Models;
using ProductOrderApi.Services;
using System.Security.Claims;

namespace ProductOrderApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;
        private readonly AppDbContext _db;

        public AuthController(IAuthService authService, AppDbContext db)
        {
            _authService = authService;
            _db = db;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
        {
            // Check if email already exists
            var existing = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email, ct);
            if (existing != null)
                return FailResponse<object>("Email already registered");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                Role = request.Role ?? "User",
                // Hash password before saving
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync(ct);

            var token = _authService.GenerateToken(user);
            return OkResponse(new { token }, "Registration successful");
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email, ct);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return FailResponse<object>("Invalid credentials");

            var token = _authService.GenerateToken(user);
            return OkResponse(new { token }, "Login successful");
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult Me()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = User.FindFirstValue(ClaimTypes.Email);
            var role = User.FindFirstValue(ClaimTypes.Role);

            return OkResponse(new { userId, email, role }, "Authenticated user info");
        }
    }

    public record LoginRequest(string Email, string Password);
    public record RegisterRequest(string Email, string Password, string? Role);
}