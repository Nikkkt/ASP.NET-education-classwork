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
            RestResponse<String> response = new()
            {
                Meta = new()
                {
                    Service = "Feedback"
                },
            };

            if (model.UserId == null || model.ProductId == null)
            {
                response.Data = "Error 400: UserId or ProductId parameter is null or empty";
                return response;
            }

            _dataContext.Feedbacks.Add(new()
            {
                UserId = model.UserId.Value,
                ProductId = model.ProductId.Value,
                Text = model.Text,
                Timestamp = model.Timestamp,
                Rate = model.Rate
            });

            await _dataContext.SaveChangesAsync();

            response.Data = "Created";
            response.Meta.Timestamp = model.Timestamp;
            return response;
        }

        [HttpPut]
        public async Task<RestResponse<String>> DoPut([FromBody] FeedbackFormModel model)
        {
            RestResponse<String> response = new()
            {
                Meta = new()
                {
                    Service = "Feedback"
                },
            };

            if (model.EditId == null)
            {
                response.Data = "Error 400: EditId parameter is null or empty";
                return response;
            }

            var feedback = _dataContext.Feedbacks.Find(model.EditId!.Value);

            if (feedback == null)
            {
                response.Data = "Error 404: Not found";
            }
            else
            {
                feedback.Text = model.Text;
                feedback.Rate = model.Rate;
                await _dataContext.SaveChangesAsync();
                response.Data = "Updated";
            }

            return response;
        }

        [HttpDelete]
        public async Task<RestResponse<String>> DoDelete([FromQuery] String id)
        {
            RestResponse<String> response = new()
            {
                Meta = new()
                {
                    Service = "Feedback"
                },
            };

            if (String.IsNullOrEmpty(id))
            {
                response.Data = "Error 400: id parameter is null or empty";
                return response;
            }

            Guid guid;
            try
            {
                guid = Guid.Parse(id);
            }
            catch
            {
                response.Data = "Error 422: id parameter is not valid UUID";
                return response;
            }

            var feedback = _dataContext.Feedbacks.Find(guid);
            if (feedback == null)
            {
                response.Data = "Error 404: id parameter does not identify feedback";
                return response;
            }

            if (feedback.DeleteDt != null)
            {
                response.Data = "Error 409: id parameter identifies already deleted feedback";
                return response;
            }

            feedback.DeleteDt = DateTime.Now;
            await _dataContext.SaveChangesAsync();
            response.Data = "Deleted";
            return response;
        }

        public async Task<RestResponse<String>> DoOther()
        {
            if (Request.Method == "RECOVERY")
            {
                return await DoRecovery();
            }
            throw new NotImplementedException();
        }

        private async Task<RestResponse<String>> DoRecovery()
        {
            RestResponse<String> response = new()
            {
                Meta = new()
                {
                    Service = "Feedback"
                },
            };

            String id = Request.Query["id"].ToString();

            if (String.IsNullOrEmpty(id))
            {
                response.Data = "Error 400: id parameter is null or empty";
                return response;
            }

            Guid guid;
            try
            {
                guid = Guid.Parse(id);
            }
            catch
            {
                response.Data = "Error 422: id parameter is not valid UUID";
                return response;
            }

            var feedback = _dataContext.Feedbacks.Find(guid);
            if (feedback == null)
            {
                response.Data = "Error 404: id parameter does not identify feedback";
                return response;
            }

            if (feedback.DeleteDt == null)
            {
                response.Data = "Error 409: id parameter identifies not deleted feedback";
                return response;
            }

            feedback.DeleteDt = null;
            await _dataContext.SaveChangesAsync();
            response.Data = "Recovered";

            return response;
        }
    }
}
