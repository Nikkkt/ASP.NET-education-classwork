using Microsoft.EntityFrameworkCore.Metadata;
using static System.Net.Mime.MediaTypeNames;

namespace ASP.NET_Classwork.Services.Upload
{
    public class FileUploadService : IFileUploader
    {
        public String UploadFile(IFormFile file, String? path)
        {
            ArgumentNullException.ThrowIfNull(file, nameof(file));

            int dotPosition = file.FileName.IndexOf('.');
            if (dotPosition == -1)
            {
                throw new ArgumentException("Файли без розширення не приймаються");
            }
            else
            {
                String ext = file.FileName[dotPosition..];
                String[] extentions = [".jpg", ".png", ".bmp"];
                if (extentions.Contains(ext))
                {
                    String filename;
                    String fullname;
                    path ??= "./Uploads/";
                    do
                    {
                        filename = Guid.NewGuid().ToString() + ext;
                        fullname = Path.Combine(path, filename);
                    } while (System.IO.File.Exists(fullname));

                    using Stream writer = new StreamWriter(fullname).BaseStream;
                    file.CopyTo(writer);
                    return filename;
                }
                else
                {
                    throw new ArgumentException("Файл має недозволене розширення");
                }
            }
        }
    }
}
