namespace ProductOrderApi.Application.Commands
{
    public class PlaceOrderCommand
    {
        public Guid UserId { get; set; }
        public List<(string SKU, int Quantity)> Items { get; set; } = new();
    }
}
