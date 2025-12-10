namespace ProductOrderApi.Domain
{
    public class DomainException : Exception
    {
        public string Code { get; }

        public DomainException(string code, string message) : base(message)
        {
            Code = code;
        }
    }
    public static class ErrorCodes
    {
        public const string InsufficientStock = "INSUFFICIENT_STOCK";
        public const string ProductNotFound = "PRODUCT_NOT_FOUND";
        public const string ConcurrencyConflict = "CONCURRENCY_CONFLICT";
    }

}
