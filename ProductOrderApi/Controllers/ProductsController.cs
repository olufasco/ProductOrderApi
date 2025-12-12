using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductOrderApi.Data;
using ProductOrderApi.DTOs;
using ProductOrderApi.Models;

namespace ProductOrderApi.Controllers
{
    [Route("api/[controller]")]
    public class ProductsController : BaseController
    {
        private readonly AppDbContext _db;
        public ProductsController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _db.Products
                .Include(p => p.Category)
                .Include(p => p.Pictures)
                .ToListAsync();

            var dto = products.Select(p => new ProductDtos.ProductDto(
                p.Id, p.Sku, p.Name, p.Description, p.Price, p.StockQuantity,
                p.CategoryId, p.Category.Name, p.Status,
                p.Pictures.Select(pic => new ProductDtos.PictureDto(pic.Url, pic.AltText, pic.MimeType, pic.SortOrder)).ToList()
            ));

            return OkResponse(dto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var product = await _db.Products
                .Include(p => p.Category)
                .Include(p => p.Pictures)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return FailResponse<ProductDtos.ProductDto>("Product not found");

            var dto = new ProductDtos.ProductDto(
                product.Id, product.Sku, product.Name, product.Description, product.Price, product.StockQuantity,
                product.CategoryId, product.Category.Name, product.Status,
                product.Pictures.Select(pic => new ProductDtos.PictureDto(pic.Url, pic.AltText, pic.MimeType, pic.SortOrder)).ToList()
            );

            return OkResponse(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductDtos.ProductCreateDto dto)
        {
            var product = new Product
            {
                Sku = dto.Sku,
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                CategoryId = dto.CategoryId,
                Status = dto.Status,
                Pictures = dto.Pictures.Select(p => new Picture
                {
                    Url = p.Url,
                    AltText = p.AltText,
                    MimeType = p.MimeType,
                    SortOrder = p.SortOrder
                }).ToList()
            };

            _db.Products.Add(product);
            await _db.SaveChangesAsync();

            return OkResponse(product, "Product created successfully");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ProductDtos.ProductUpdateDto dto)
        {
            var product = await _db.Products.Include(p => p.Pictures).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return FailResponse<Product>("Product not found");

            product.Sku = dto.Sku;
            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.StockQuantity = dto.StockQuantity;
            product.CategoryId = dto.CategoryId;
            product.Status = dto.Status;

            product.Pictures = dto.Pictures.Select(p => new Picture
            {
                Url = p.Url,
                AltText = p.AltText,
                MimeType = p.MimeType,
                SortOrder = p.SortOrder
            }).ToList();

            await _db.SaveChangesAsync();
            return OkResponse(product, "Product updated successfully");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return FailResponse<Product>("Product not found");

            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
            return OkResponse(product, "Product deleted successfully");
        }
    }
}