namespace ProductOrderApi.Models
{
    public class User : BaseEntity
    {
        public string Email { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public string Role { get; set; } = "User";
    }
}