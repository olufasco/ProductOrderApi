using ProductOrderApi.Application.DTOs;

namespace ProductOrderApi.Application.Commands
{
    public class AddToCartCommand
    {
        public Guid UserId { get; set; }
        public AddToCartDto Dto { get; set; } = new();
    }
}
