using System.Threading.Tasks;

namespace CodePanthers.AWS.S3.UploadService.Services
{
    public interface IUploadService
    {
        void UploadFileToS3();
        Task<string> GetS3ObjectUrl();
    }
}
