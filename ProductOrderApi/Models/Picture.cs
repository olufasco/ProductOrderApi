namespace ProductOrderApi.Models
{
    public class Picture : BaseEntity
    {
        public string Url { get; set; } = default!;
        public string? AltText { get; set; }
        public string? MimeType { get; set; }
        public int SortOrder { get; set; }
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = default!;
    }

}
