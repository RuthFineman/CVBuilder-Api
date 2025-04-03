using Amazon.S3;
using CVBuilder.Core.DTOs;
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
        public async Task<IActionResult> UploadFile(IFormFile file, [FromQuery] string userId, [FromForm] FileCVDto fileDto)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            try
            {
                // העברת כל המידע כולל ה-DTO
                await _fileUploadService.UploadFileAsync(file, userId, fileDto);
                return Ok(new { message = "File uploaded successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error uploading file: {ex.Message}");
            }
        }
    }
}
