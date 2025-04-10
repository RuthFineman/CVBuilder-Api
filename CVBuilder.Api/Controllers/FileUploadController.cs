using Amazon.S3;
using CVBuilder.Core.DTOs;
using CVBuilder.Core.Services;
using CVBuilder.Data;
using CVBuilder.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Swashbuckle.AspNetCore.Annotations;
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
        [Authorize]
        [HttpGet("user-files")]
        public async Task<IActionResult> GetUserFiles(string userId)
        {
            try
            {
                var files = await _fileUploadService.GetUserFilesAsync(userId);
                return Ok(files); // מחזיר [{ id: 3, path: "115/3.pdf" }, ...]
            }
            catch (Exception ex)
            {
                return StatusCode(500, "שגיאה בטעינת הקבצים: " + ex.Message);
            }
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
            // קבלת userId מתוך ה-Context
            var userId = GetUserIdFromContext().ToString();

            // קריאה לפונקציה שתמחק את הקובץ עבור ה-userId הספציפי
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
        [HttpPut("update/{id}")]
        [SwaggerOperation(Summary = "Update resume with file upload", Description = "Allows updating a resume with the file and other details.")]
        public async Task<IActionResult> UpdateFileCV(IFormFile file, int id, [FromForm] FileCVDto fileCVDto)
        {
            var userId = GetUserIdFromContext().ToString();
            var result = await _fileUploadService.UpdateFileCVAsync(file, id, userId, fileCVDto);

            if (result == null)
                return NotFound("Resume not found.");
            return Ok(result);
        }
        //לעדכון
        [HttpGet("fileCV/{id}")]
        public async Task<IActionResult> GetFileCV([FromRoute] int id)
        {
            try
            {
                var userId = GetUserIdFromContext().ToString();
                if (string.IsNullOrEmpty(userId) || userId == Guid.Empty.ToString())
                    return BadRequest("User ID is missing or invalid");

                var file = await _fileUploadService.GetFileCVByIdAsync(id, userId);

                if (file == null)
                    return NotFound();

                return Ok(file);
            }
            catch (Exception ex)
            {
                // כדאי לרשום ללוג - כאן רק נחזיר את השגיאה למטרת איתור
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("check-filename")]
        public IActionResult CheckIfFileExists([FromQuery] string userId, [FromQuery] string fileName)
        {
            var userFolderPath = $"users/{userId}/";
            var fullKey = $"{userFolderPath}{fileName}";

            // בדיקה ב-S3
            var exists = _fileUploadService.DoesFileExist(fullKey); // את צריכה לממש את הפונקציה הזו

            return Ok(new { exists });
        }
    }
}
