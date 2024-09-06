namespace ASP.NET_Classwork.Data.Entities
{
    public class Product
    {
        public Guid                   Id            { get; set; }
        public Guid                   GroupId       { get; set; }
        public String                 Name          { get; set; } = null!;
        public String?                Description   { get; set; }
        public String?                Picture       { get; set; }
        public double                 Price         { get; set; }
        public long                   Amount        { get; set; }
        public DateTime?              DeleteDt      { get; set; }
        public ProductGroup           Group         { get; set; }
        public String?                Slug          { get; set; }
        public IEnumerable<Feedback>  Feedbacks     { get; set; } = null!;
    }
}
