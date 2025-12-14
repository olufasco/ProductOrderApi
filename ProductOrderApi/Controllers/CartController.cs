using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductOrderApi.Application.DTOs;
using ProductOrderApi.Domain.Entities;
using System.Security.Claims;

namespace ProductOrderApi.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public CartController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // Get the current user's ID from JWT
        private Guid GetUserId() => Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException("User ID not found"));

        // Get the current user's cart
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var cart = await _uow.Carts.GetByUserIdAsync(GetUserId());
            if (cart == null)
                return Ok(ApiResponse<CartDto?>.Ok(null, "Cart is empty"));

            var dto = new CartDto
            {
                Id = cart.Id,
                Items = cart.Items.Select(i => new CartItemDto
                {
                    ProductId = i.ProductId,
                    SKU = i.SKU,
                    Quantity = i.Quantity
                }).ToList()
            };

            return Ok(ApiResponse<CartDto>.Ok(dto));
        }

        // Add product to cart
        [HttpPost("add")]
        public async Task<IActionResult> AddToCart(AddToCartDto dto)
        {
            var userId = GetUserId();
            var cart = await _uow.Carts.GetByUserIdAsync(userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                await _uow.Carts.AddAsync(cart);
                await _uow.SaveChangesAsync();
            }

            var product = await _uow.Products.GetBySkuAsync(dto.SKU);
            if (product == null)
                return BadRequest(ApiResponse<string>.Fail("Product not found"));

            if (product.StockQuantity < dto.Quantity)
                return BadRequest(ApiResponse<string>.Fail("Not enough stock"));

            var existingItem = await _uow.Carts.GetCartItemAsync(cart.Id, dto.SKU);
            if (existingItem != null)
            {
                existingItem.Quantity += dto.Quantity;
                _uow.Carts.UpdateCartItem(existingItem);
            }
            else
            {
                await _uow.DbContext.CartItems.AddAsync(new CartItem
                {
                    CartId = cart.Id,
                    ProductId = product.Id,
                    SKU = dto.SKU,
                    Quantity = dto.Quantity
                });
            }

            await _uow.SaveChangesAsync();
            return Ok(ApiResponse<string>.Ok("Product added to cart"));
        }

        // Remove a single item from cart
        [HttpDelete("item/{sku}")]
        public async Task<IActionResult> DeleteCartItem(string sku)
        {
            var userId = GetUserId();
            var cart = await _uow.Carts.GetByUserIdAsync(userId);
            if (cart == null)
                return NotFound(ApiResponse<string>.Fail("Cart not found"));

            var item = cart.Items.FirstOrDefault(i => i.SKU == sku);
            if (item == null)
                return NotFound(ApiResponse<string>.Fail("Item not found in cart"));

            _uow.Carts.DeleteCartItem(item);
            await _uow.SaveChangesAsync();

            return Ok(ApiResponse<string>.Ok($"Item {sku} removed from cart"));
        }

        // Clear all items from cart
        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetUserId();
            var cart = await _uow.Carts.GetByUserIdAsync(userId);
            if (cart == null || !cart.Items.Any())
                return Ok(ApiResponse<string>.Ok("Cart is already empty"));

            _uow.DbContext.CartItems.RemoveRange(cart.Items);
            await _uow.SaveChangesAsync();

            return Ok(ApiResponse<string>.Ok("Cart cleared successfully"));
        }

        // Checkout
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout()
        {
            var userId = GetUserId();
            var cart = await _uow.Carts.GetByUserIdAsync(userId);
            if (cart == null || !cart.Items.Any())
                return BadRequest(ApiResponse<string>.Fail("Cart is empty"));

            using var transaction = await _uow.DbContext.Database.BeginTransactionAsync();

            var order = new Order { UserId = userId };
            decimal total = 0;

            foreach (var item in cart.Items)
            {
                var product = await _uow.Products.GetBySkuAsync(item.SKU);
                if (product == null || product.StockQuantity < item.Quantity)
                    return BadRequest(ApiResponse<string>.Fail($"Product {item.SKU} out of stock"));

                product.StockQuantity -= item.Quantity;
                _uow.Products.Update(product);

                order.Items.Add(new OrderItem
                {
                    ProductId = product.Id,
                    SKU = item.SKU,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                });

                total += item.Quantity * product.Price;
            }

            order.TotalAmount = total;
            await _uow.Orders.AddAsync(order);

            // Clear cart items
            _uow.DbContext.CartItems.RemoveRange(cart.Items);

            await _uow.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(ApiResponse<Guid>.Ok(order.Id, "Order placed successfully"));
        }
    }
}
