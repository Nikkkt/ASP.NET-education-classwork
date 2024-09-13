namespace ASP.NET_Classwork.Models.Api
{
    public class FeedbackFormModel
    {
        public Guid? ProductId { get; set; }
        public Guid? UserId { get; set; }
        public Guid? EditId { get; set; }
        public String Text { get; set; }
        public long Timestamp { get; set; } = DateTime.Now.Ticks;
        public int Rate { get; set; } = 5;

    }
}
