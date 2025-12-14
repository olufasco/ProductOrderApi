using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using ProductOrderApi.Domain.Entities;

namespace ProductOrderApi.Application.Commands.Handlers
{
    public class LoginUserHandler
    {
        private readonly IUnitOfWork _uow;
        private readonly IConfiguration _config;

        public LoginUserHandler(IUnitOfWork uow, IConfiguration config)
        {
            _uow = uow;
            _config = config;
        }

        public async Task<ApiResponse<string>> Handle(LoginUserCommand command)
        {
            var user = await _uow.Users.GetByEmailAsync(command.Dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(command.Dto.Password, user.PasswordHash))
                return ApiResponse<string>.Fail("Invalid credentials");

            var token = GenerateJwtToken(user);
            return ApiResponse<string>.Ok(token, "Login successful");
        }

        private string GenerateJwtToken(User user)
        {
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Secret"] ?? "SuperSecretKey123!");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
