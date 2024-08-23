namespace ASP.NET_Classwork.Services.Upload
{
    public interface IFileUploader
    {
        String UploadFile(IFormFile file, String path);
    }
}
