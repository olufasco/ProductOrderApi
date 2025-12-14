using ProductOrderApi.Application.DTOs;

namespace ProductOrderApi.Application.Commands
{
    public class LoginUserCommand
    {
        public LoginUserDto Dto { get; set; } = new();
    }
}
