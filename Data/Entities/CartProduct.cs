using System.Text.Json.Serialization;

namespace ASP.NET_Classwork.Data.Entities
{
    public class CartProduct
    {
        public Guid Id { get; set; }
        public Guid CartId { get; set; }
        public Guid ProductId { get; set; }
        public int Count { get; set; } = 1;

        // Навігаційні властивості
        [JsonIgnore]
        public Cart Cart { get; set; }
        public Product Product { get; set; }
    }
}
