using ProductOrderApi.Abstractions;
using ProductOrderApi.Abstractions.ProductOrder.Application.Abstractions;
using ProductOrderApi.Models;
using static ProductOrderApi.DTOs.ProductDtos;

namespace ProductOrderApi.Services
{        public class ProductService
        {
            private readonly IProductRepository _repo;
            private readonly IUnitOfWork _uow;

            public ProductService(IProductRepository repo, IUnitOfWork uow)
            {
                _repo = repo;
                _uow = uow;
            }

            public async Task<List<ProductDto>> GetAllAsync(CancellationToken ct)
            {
                var products = await _repo.GetAllAsync(ct);
                return products.Select(p => new ProductDto(p.Id, p.Name, p.Description, p.Price, p.StockQuantity)).ToList();
            }

            public async Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken ct)
            {
                var product = await _repo.GetByIdAsync(id, ct);
                return product is null ? null : new ProductDto(product.Id, product.Name, product.Description, product.Price, product.StockQuantity);
            }

            public async Task<ProductDto> CreateAsync(ProductCreateDto dto, CancellationToken ct)
            {
                var product = new Product
                {
                    Id = Guid.NewGuid(),
                    Name = dto.Name,
                    Description = dto.Description,
                    Price = dto.Price,
                    StockQuantity = dto.StockQuantity
                };

                await _repo.AddAsync(product, ct);
                await _uow.SaveChangesAsync(ct);

                return new ProductDto(product.Id, product.Name, product.Description, product.Price, product.StockQuantity);
            }

            public async Task<ProductDto?> UpdateAsync(Guid id, ProductUpdateDto dto, CancellationToken ct)
            {
                var product = await _repo.GetByIdAsync(id, ct);
                if (product is null) return null;

                product.Name = dto.Name;
                product.Description = dto.Description;
                product.Price = dto.Price;
                product.StockQuantity = dto.StockQuantity;

                await _repo.UpdateAsync(product, ct);
                await _uow.SaveChangesAsync(ct);

                return new ProductDto(product.Id, product.Name, product.Description, product.Price, product.StockQuantity);
            }

            public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
            {
                var product = await _repo.GetByIdAsync(id, ct);
                if (product is null) return false;

                await _repo.SoftDeleteAsync(product, "system", ct);
                await _uow.SaveChangesAsync(ct);
                return true;
            }
        }
    }

