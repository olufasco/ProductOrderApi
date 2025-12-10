namespace ProductOrderApi.Abstractions
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
    }
}