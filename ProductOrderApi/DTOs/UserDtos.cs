namespace ProductOrderApi.DTOs
{
    public class UserDtos
    {
        public record RegisterDto(string Email, string Password, string Role = "User");
        public record LoginDto(string Email, string Password);
        public record UserDto(Guid Id, string Email, string Role);
    }
}