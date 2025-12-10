using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductOrderApi.Services;
using static ProductOrderApi.DTOs.ProductDtos;

namespace ProductOrderApi.Controllers
{
        [ApiController]
        [Route("api/products")]
        public class ProductsController : ControllerBase
        {
            private readonly ProductService _service;

            public ProductsController(ProductService service)
            {
                _service = service;
            }

            // GET: api/products
            [HttpGet]
            public async Task<ActionResult<List<ProductDto>>> GetAll(CancellationToken ct) =>
                Ok(await _service.GetAllAsync(ct));

            // GET: api/products/{id}
            [HttpGet("{id:guid}")]
            public async Task<ActionResult<ProductDto>> Get(Guid id, CancellationToken ct)
            {
                var product = await _service.GetByIdAsync(id, ct);
                return product is null ? NotFound() : Ok(product);
            }

            // POST: api/products
            [HttpPost]
            public async Task<ActionResult<ProductDto>> Create(ProductCreateDto dto, CancellationToken ct)
            {
                var created = await _service.CreateAsync(dto, ct);
                return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
            }

            // PUT: api/products/{id}
            [HttpPut("{id:guid}")]
            public async Task<ActionResult<ProductDto>> Update(Guid id, ProductUpdateDto dto, CancellationToken ct)
            {
                var updated = await _service.UpdateAsync(id, dto, ct);
                return updated is null ? NotFound() : Ok(updated);
            }

            // DELETE: api/products/{id}
            [HttpDelete("{id:guid}")]
            public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
            {
                var deleted = await _service.DeleteAsync(id, ct);
                return deleted ? NoContent() : NotFound();
            }
        }
}