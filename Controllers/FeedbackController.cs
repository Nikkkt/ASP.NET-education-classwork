using ASP.NET_Classwork.Data;
using ASP.NET_Classwork.Data.Entities;
using ASP.NET_Classwork.Models.Api;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASP.NET_Classwork.Controllers
{
    [Route("api/feedback")]
    [ApiController]
    public class FeedbackController(DataContext dataContext) : ControllerBase
    {
        private readonly DataContext _dataContext = dataContext;

        [HttpGet]
        public async Task<RestResponse<List<Feedback>>> DoGet()
        {
            List<Feedback> feedbacks = await _dataContext.Feedbacks.ToListAsync();
            return new()
            {
                Meta = new()
                {
                    Service = "Feedback",
                    Count = feedbacks.Count
                },
                Data = feedbacks
            };
        }

        [HttpPost]
        public async Task<RestResponse<String>> DoPost([FromBody] FeedbackFormModel model)
        {
            _dataContext.Feedbacks.Add(new()
            {
                UserId = model.UserId,
                ProductId = model.ProductId,
                Text = model.Text,
                Timestamp = model.Timestamp
            });

            await _dataContext.SaveChangesAsync();

            return new()
            {
                Meta = new()
                {
                    Service = "Feedback",
                    Timestamp = model.Timestamp
                },
                Data = "Created"
            };
        }
    }
}
