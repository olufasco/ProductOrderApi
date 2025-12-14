using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductOrderApi.Application.DTOs;
using ProductOrderApi.Domain.Entities;
using ProductOrderApi.ProductOrderApi.Application.DTOs;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IUnitOfWork _uow;

    public ProductsController(IUnitOfWork uow) => _uow = uow;

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddProduct(ProductDto dto)
    {
        var category = await _uow.Categories.GetByIdAsync(dto.CategoryId);
        if (category == null) return BadRequest(ApiResponse<string>.Fail("Invalid CategoryId"));

        var existingSku = await _uow.Products.GetBySkuAsync(dto.SKU);
        if (existingSku != null) return BadRequest(ApiResponse<string>.Fail("SKU already exists"));

        var product = new Product
        {
            SKU = dto.SKU,
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            PictureUrl = dto.PictureUrl,
            CategoryId = dto.CategoryId
        };

        await _uow.Products.AddAsync(product);
        await _uow.SaveChangesAsync();

        return Ok(ApiResponse<Guid>.Ok(product.Id, "Product created"));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _uow.Products.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<Product>>.Ok(products));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var product = await _uow.Products.GetByIdAsync(id);
        return product != null
            ? Ok(ApiResponse<Product>.Ok(product))
            : NotFound(ApiResponse<string>.Fail("Product not found"));
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, ProductDto dto)
    {
        var product = await _uow.Products.GetByIdAsync(id);
        if (product == null) return NotFound(ApiResponse<string>.Fail("Product not found"));

        var category = await _uow.Categories.GetByIdAsync(dto.CategoryId);
        if (category == null) return BadRequest(ApiResponse<string>.Fail("Invalid CategoryId"));

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.StockQuantity = dto.StockQuantity;
        product.PictureUrl = dto.PictureUrl;
        product.CategoryId = dto.CategoryId;

        _uow.Products.Update(product);
        await _uow.SaveChangesAsync();

        return Ok(ApiResponse<string>.Ok("Product updated"));
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        var product = await _uow.Products.GetByIdAsync(id);
        if (product == null) return NotFound(ApiResponse<string>.Fail("Product not found"));

        _uow.Products.Delete(product);
        await _uow.SaveChangesAsync();

        return Ok(ApiResponse<string>.Ok("Product deleted"));
    }
}
