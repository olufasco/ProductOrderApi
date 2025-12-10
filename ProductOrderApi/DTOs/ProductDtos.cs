namespace ProductOrderApi.DTOs
{
    public class ProductDtos
    {
        public record ProductCreateDto(string Name, string Description, decimal Price, int StockQuantity);
        public record ProductUpdateDto(string Name, string Description, decimal Price, int StockQuantity);
        public record ProductDto(Guid Id, string Name, string Description, decimal Price, int StockQuantity);
    }
}
