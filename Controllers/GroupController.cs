using ASP.NET_Classwork.Data;
using ASP.NET_Classwork.Models.Group;
using ASP.NET_Classwork.Services.Upload;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP_P15.Controllers
{
    [Route("api/group")]
    [ApiController]
    public class GroupController(IFileUploader fileUploader, DataContext dataContext) : ControllerBase
    {
        private readonly IFileUploader _fileUploader = fileUploader;
        private readonly DataContext _dataContext = dataContext;

        [HttpPost]
        public async Task<object> DoPost(GroupFormModel formModel)
        {
            String uploadedName;
            try
            {
                uploadedName = _fileUploader.UploadFile(formModel.ImageFile, "./Uploads/Shop");

                _dataContext.Groups.Add(
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Name = formModel.Name,
                        Description = formModel.Description,
                        Image = uploadedName,
                        DeleteDt = null,
                        Slug = formModel.Slug
                    });

                await _dataContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new
                {
                    status = "error",
                    code = 500,
                    message = ex.Message
                };
            }
            return new
            {
                status = "OK",
                code = 200,
                message = "Created"
            };
        } 
    }
}
