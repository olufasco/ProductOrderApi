using ProductOrderApi.Application.DTOs;
namespace ProductOrderApi.Application.Commands
{
    public class RegisterUserCommand
    {
        public RegisterUserDto Dto { get; set; } = new();
    }
}
