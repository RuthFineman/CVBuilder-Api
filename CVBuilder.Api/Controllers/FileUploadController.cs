using Amazon.S3;
using CVBuilder.Core.DTOs;
using CVBuilder.Core.Services;
using CVBuilder.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        private int GetUserIdFromContext()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null)
            {
                return int.Parse(userIdClaim.Value);
            }
            throw new UnauthorizedAccessException("User not authenticated.");
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
        [Authorize]
        [HttpDelete("remove/{id}")]
        public async Task<IActionResult> DeleteFile(int id)
        {
            var userId = GetUserIdFromContext();
            var result = await _fileUploadService.DeleteFileByUserIdAsync(id, userId);
            if (result)
            {
                return Ok(new { message = "File deleted successfully." });
            }
            else
            {
                return Unauthorized(new { message = "File not found or doesn't belong to the user." });
            }
        }
    }
}
