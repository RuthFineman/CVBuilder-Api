using Amazon.S3;
using CVBuilder.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace CVBuilder.Api.Controllers
{
    [Route("upload")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly IFileUploadService _fileUploadService;

        public FileUploadController(IFileUploadService fileUploadService)
        {
            _fileUploadService = fileUploadService;
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("לא נבחר קובץ להעלות.");

            try
            {
                // שולחים את הקובץ ל-Service שיטפל בהעלאה
                await _fileUploadService.UploadFileAsync(file); // שינוי כאן

                return Ok(new { message = "הקובץ הועלה בהצלחה" });
            }
            catch (AmazonS3Exception s3Ex)
            {
                // לוג של השגיאה
                Console.WriteLine($"S3 Error: {s3Ex.Message}");
                throw new Exception($"שגיאה בהעלאת הקובץ ל-S3: {s3Ex.Message}", s3Ex);
            }

        }
    }
}
