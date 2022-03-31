using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace CodePanthers.AWS.S3.UploadService.Services
{
    public interface IFileUploadService
    {
       Task<string> GetS3ObjectPresignedUrl(IFormFile file);
    }
}
