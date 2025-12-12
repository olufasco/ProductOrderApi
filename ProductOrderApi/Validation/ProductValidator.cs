using FluentValidation;
using static ProductOrderApi.DTOs.ProductDtos;

namespace ProductOrderApi.Validation
{
    public class PictureValidator : AbstractValidator<PictureDto>
    {
        public PictureValidator()
        {
            RuleFor(x => x.Url).NotEmpty().MaximumLength(2048);
            RuleFor(x => x.SortOrder).GreaterThanOrEqualTo(0);
        }
    }
    public class ProductCreateValidator : AbstractValidator<ProductCreateDto>
    {
        public ProductCreateValidator()
        {
            RuleFor(x => x.Sku).NotEmpty().MaximumLength(64);
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
            RuleFor(x => x.Price).GreaterThan(0);
            RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0);
            RuleFor(x => x.CategoryId).NotEmpty();
            RuleFor(x => x.Status).IsInEnum();

            RuleForEach(x => x.Pictures).SetValidator(new PictureValidator());
        }
    }
    public class ProductUpdateValidator : AbstractValidator<ProductUpdateDto>
    {
        public ProductUpdateValidator()
        {
            RuleFor(x => x.Id).NotEmpty(); 
            RuleFor(x => x.Sku).NotEmpty().MaximumLength(64);
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
            RuleFor(x => x.Price).GreaterThan(0);
            RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0);
            RuleFor(x => x.CategoryId).NotEmpty();
            RuleFor(x => x.Status).IsInEnum();
            RuleForEach(x => x.Pictures).SetValidator(new PictureValidator());
        }
    }
}