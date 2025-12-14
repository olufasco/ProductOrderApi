using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductOrderApi.Application.DTOs;
using ProductOrderApi.Domain.Entities;

[ApiController]
[Route("api/categories")]
[Authorize]
public class CategoryController : ControllerBase
{
    private readonly IUnitOfWork _uow;

    public CategoryController(IUnitOfWork uow) => _uow = uow;

    [HttpPost]
    public async Task<IActionResult> AddCategory(CategoryDto dto)
    {
        var category = new Category { Name = dto.Name };
        await _uow.Categories.AddAsync(category);
        await _uow.SaveChangesAsync();

        return Ok(ApiResponse<Guid>.Ok(category.Id, "Category created successfully"));
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _uow.Categories.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<Category>>.Ok(categories));
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
    {
        var category = await _uow.Categories.GetByIdAsync(id);
        if (category == null)
            return NotFound(ApiResponse<string>.Fail("Category not found"));

        return Ok(ApiResponse<Category>.Ok(category));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, CategoryDto dto)
    {
        var category = await _uow.Categories.GetByIdAsync(id);
        if (category == null)
            return NotFound(ApiResponse<string>.Fail("Category not found"));

        category.Name = dto.Name;
        _uow.Categories.Update(category);
        await _uow.SaveChangesAsync();

        return Ok(ApiResponse<string>.Ok("Category updated successfully"));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var category = await _uow.Categories.GetByIdAsync(id);
        if (category == null)
            return NotFound(ApiResponse<string>.Fail("Category not found"));

        _uow.Categories.Delete(category);
        await _uow.SaveChangesAsync();

        return Ok(ApiResponse<string>.Ok("Category deleted successfully"));
    }
}
