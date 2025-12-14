using ProductOrderApi.Domain.Entities;

namespace ProductOrderApi.Application.Commands.Handlers
{
    public class CreateProductHandler
    {
        private readonly IUnitOfWork _uow;
        public CreateProductHandler(IUnitOfWork uow) => _uow = uow;

        public async Task<ApiResponse<Guid>> Handle(CreateProductCommand command)
        {
            var category = await _uow.Categories.GetByIdAsync(command.Dto.CategoryId);
            if (category == null)
                return ApiResponse<Guid>.Fail("Invalid category");

            var existingSku = await _uow.Products.GetBySkuAsync(command.Dto.SKU);
            if (existingSku != null)
                return ApiResponse<Guid>.Fail("SKU already exists");

            var product = new Product
            {
                SKU = command.Dto.SKU,
                Name = command.Dto.Name,
                Description = command.Dto.Description,
                Price = command.Dto.Price,
                StockQuantity = command.Dto.StockQuantity,
                PictureUrl = command.Dto.PictureUrl,
                CategoryId = command.Dto.CategoryId
            };

            await _uow.Products.AddAsync(product);
            await _uow.SaveChangesAsync();

            return ApiResponse<Guid>.Ok(product.Id, "Product created successfully");
        }
    }
}
