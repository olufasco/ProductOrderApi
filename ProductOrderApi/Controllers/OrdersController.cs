using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductOrderApi.Domain;
using ProductOrderApi.Services;
using static ProductOrderApi.DTOs.OrderDtos;

namespace ProductOrderApi.Controllers
{
        [ApiController]
        [Route("api/orders")]
        public class OrdersController : ControllerBase
        {
            private readonly OrderService _service;

            public OrdersController(OrderService service)
            {
                _service = service;
            }

            // GET: api/orders/{id}
            [HttpGet("{id:guid}")]
            public async Task<ActionResult<OrderDto>> Get(Guid id, CancellationToken ct)
            {
                var order = await _service.GetByIdAsync(id, ct);
                return order is null ? NotFound() : Ok(order);
            }

            // GET: api/orders
            [HttpGet]
            public async Task<ActionResult<List<OrderDto>>> GetAll(CancellationToken ct) =>
                Ok(await _service.GetAllAsync(ct));

            // POST: api/orders/place
            [HttpPost("place")]
            public async Task<ActionResult<OrderDto>> Place(PlaceOrderRequest request, CancellationToken ct)
            {
                try
                {
                    var result = await _service.PlaceOrderAsync(request, ct);
                    return Ok(result);
                }
                catch (DomainException ex) when (ex.Code == ErrorCodes.InsufficientStock)
                {
                    return Problem(ex.Message, statusCode: StatusCodes.Status409Conflict, title: "Insufficient Stock");
                }
            }

            // DELETE: api/orders/{id}
            [HttpDelete("{id:guid}")]
            public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
            {
                var deleted = await _service.DeleteAsync(id, ct);
                return deleted ? NoContent() : NotFound();
            }
        }
}
