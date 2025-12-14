using BCrypt.Net;
using ProductOrderApi.Domain.Entities;

namespace ProductOrderApi.Application.Commands.Handlers
{
    public class RegisterUserHandler
    {
        private readonly IUnitOfWork _uow;
        public RegisterUserHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<ApiResponse<Guid>> Handle(RegisterUserCommand command)
        {
            var existing = await _uow.Users.GetByEmailAsync(command.Dto.Email);
            if (existing != null)
                return ApiResponse<Guid>.Fail("Email already exists");

            var user = new User
            {
                Email = command.Dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.Dto.Password)
            };

            await _uow.Users.AddAsync(user);
            await _uow.SaveChangesAsync();

            return ApiResponse<Guid>.Ok(user.Id, "User registered successfully");
        }
    }
}
