using ProductOrderApi.Models;

namespace ProductOrderApi.DTOs
{
    public class ProductDtos
    {
        public record PictureDto(string Url, string? AltText, string? MimeType, int SortOrder);
        public record ProductCreateDto(
            string Sku,
            string Name,
            string Description,
            decimal Price,
            int StockQuantity,
            Guid CategoryId,
            ProductStatus Status,
            List<PictureDto> Pictures
        );
        public record ProductUpdateDto(
            Guid Id,
            string Sku,
            string Name,
            string Description,
            decimal Price,
            int StockQuantity,
            Guid CategoryId,
            ProductStatus Status,
            List<PictureDto> Pictures
        );
        public record ProductDto(
            Guid Id,
            string Sku,
            string Name,
            string Description,
            decimal Price,
            int StockQuantity,
            Guid CategoryId,
            string CategoryName,
            ProductStatus Status,
            List<PictureDto> Pictures
        );
    }
}