namespace ASP.NET_Classwork.Models.Cart
{
    public class CartFormModel
    {
        public Guid ProductId { get; set; }
        public Guid UserId { get; set; }
        public int Count { get; set; }
    }
}
