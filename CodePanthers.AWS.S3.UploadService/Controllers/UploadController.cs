using CodePanthers.AWS.S3.UploadService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CodePanthers.AWS.S3.UploadService.Controllers
{
    [Route("Upload")]
    public class UploadController : Controller
    {
        private readonly IFileUploadService _fileUploadService;
        public UploadController(IFileUploadService fileUploadService)
        {
            _fileUploadService = fileUploadService;
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromForm] IFormFile file)
        {
            var presignedUrl = await _fileUploadService.GetS3ObjectPresignedUrl(file);
            if (presignedUrl != null)
                return Ok(new { status = "Success", url = presignedUrl });
            else
                return BadRequest(new { status = "Failed", message = "Something went wrong" });
        }
    }
}
