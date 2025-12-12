using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductOrderApi.Data;
using ProductOrderApi.Models;

namespace ProductOrderApi.Controllers
{
    [Route("api/[controller]")]
    public class CategoriesController : BaseController
    {
        private readonly AppDbContext _db;
        public CategoriesController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _db.Categories.Include(c => c.Products).ToListAsync();
            return OkResponse(categories);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            _db.Categories.Add(category);
            await _db.SaveChangesAsync();
            return OkResponse(category, "Category created successfully");
        }
    }
}