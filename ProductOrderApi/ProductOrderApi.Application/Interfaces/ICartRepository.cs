using ProductOrderApi.Domain.Entities;
public interface ICartRepository
{
    Task<Cart?> GetByUserIdAsync(Guid userId);
    Task<CartItem?> GetCartItemAsync(Guid cartId, string sku);
    Task AddAsync(Cart cart);
    Task AddCartItemAsync(CartItem item);
    void UpdateCartItem(CartItem item);
    void DeleteCartItem(CartItem item);
    void RemoveCartItem(CartItem item);
    void UpdateCart(Cart cart);
}