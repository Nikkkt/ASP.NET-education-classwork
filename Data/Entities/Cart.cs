namespace ASP.NET_Classwork.Data.Entities
{
    public class Cart
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreateDt { get; set; }
        public DateTime? DeleteDt { get; set; }
        public DateTime? CloseDt { get; set; }

        public IEnumerable<CartProduct> CartProducts { get; set; }
    }
}
