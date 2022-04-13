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
        private readonly INotificationService _notificationService;
        public UploadController(IFileUploadService fileUploadService, INotificationService notificationService)
        {
            _fileUploadService = fileUploadService;
            _notificationService = notificationService;
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

        [HttpPost("register")]
        public async Task<IActionResult> RegisterEmailSubscription([FromBody] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(new { status = "Failed", message = "Enter a valid Email Address" });
            }
            else
            {
                await _notificationService.RegisterSubscirption(email);
                return Ok(new { status = "success", message = "Email succesfully registered" });
            }
        }
    }
}
