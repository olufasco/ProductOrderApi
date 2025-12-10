using FluentValidation;
using static ProductOrderApi.DTOs.ProductDtos;

namespace ProductOrder.Application.Validation
{
    public class ProductCreateValidator : AbstractValidator<ProductCreateDto>
    {
        public ProductCreateValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
            RuleFor(x => x.Price).GreaterThan(0);
            RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0);
        }
    }

    public class ProductUpdateValidator : AbstractValidator<ProductUpdateDto>
    {
        public ProductUpdateValidator()
        {
            Include(new ProductCreateValidator()); 
        }

        private void Include(ProductCreateValidator productCreateValidator)
        {
            throw new NotImplementedException();
        }
    }
}