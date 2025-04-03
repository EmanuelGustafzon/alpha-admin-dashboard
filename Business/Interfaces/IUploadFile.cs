using Microsoft.AspNetCore.Http;

namespace Business.Interfaces;
public interface IUploadFile
{
    public Task<string> UploadFileLocally(IFormFile file);
}
