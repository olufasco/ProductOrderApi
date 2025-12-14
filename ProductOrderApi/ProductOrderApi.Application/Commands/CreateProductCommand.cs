using ProductOrderApi.Application.DTOs;

namespace ProductOrderApi.Application.Commands
{
    public class CreateProductCommand
    {
        public ProductDto Dto { get; set; } = new();
    }
}
