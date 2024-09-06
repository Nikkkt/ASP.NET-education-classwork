namespace ASP.NET_Classwork.Data.Entities
{
    public class ProductGroup
    {
        public Guid                   Id            { get; set; }
        public String                 Name          { get; set; }
        public String                 Description   { get; set; }
        public String?                Image         { get; set; }
        public DateTime?              DeleteDt      { get; set; }
        public String?                Slug          { get; set; }
        public IEnumerable<Product>   Products      { get; set; } = null!;
    }
}
