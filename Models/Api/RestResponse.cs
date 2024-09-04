namespace ASP.NET_Classwork.Models.Api
{
    public class RestResponse<T>
    {
        public MetaData Meta {  get; set; }
        public T Data { get; set; }
    }

    public class MetaData
    {
        public string Service { get; set; }
        public long Timestamp { get; set; } = DateTime.Now.Ticks;
        public int? Count { get; set; }
    }
}
