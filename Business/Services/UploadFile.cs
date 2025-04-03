using Business.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Business.Services;

public class UploadFile(IWebHostEnvironment webHostEnv) : IUploadFile
{
    private readonly IWebHostEnvironment _webHostEnv = webHostEnv;

    public async Task<string> UploadFileLocally(IFormFile file)
    {
        if (file != null && file.Length > 0)
        {
            var folderName = "uploads";
            var path = Path.Combine(_webHostEnv.WebRootPath, folderName);
            Directory.CreateDirectory(path);

            var fileName = $"{Guid.NewGuid().ToString()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(path, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return $"{folderName}/{fileName}";
        }

        return "";
    }
}
