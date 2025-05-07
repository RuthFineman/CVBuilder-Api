using Amazon.S3;
using CVBuilder.Core.DTOs;
using CVBuilder.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Swashbuckle.AspNetCore.Annotations;
using DTO = CVBuilder.Core.DTOs;
using Newtonsoft.Json;
using CVBuilder.Core.Models;

namespace CVBuilder.Api.Controllers
{
    [Route("upload")]
    [ApiController]
    public class FileCVController : ControllerBase
    {
        private readonly IFileCVService _fileUploadService;

        public FileCVController(IFileCVService fileUploadService)
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
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file, [FromQuery] string userId)
        {
            Console.WriteLine($"File upload started: {DateTime.Now}");
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }
            try
            {
                var userFileCount = await _fileUploadService.GetFileCountByUserIdAsync(userId);
                if (userFileCount >= 5)
                {
                    return BadRequest("ניתן לשמור עד 5 קורות חיים בלבד למשתמש.");
                }
                // קריאה מה-Form של כל שדה כטקסט
                var title = Request.Form["Title"];
                var description = Request.Form["Description"];
                var workExperiencesJson = Request.Form["WorkExperiences"];
                var educationsJson = Request.Form["Educations"];
                var languagesJson = Request.Form["Languages"];

                // המרות JSON
                var workExperiences = JsonConvert.DeserializeObject<List<DTO.WorkExperience>>(workExperiencesJson);
                var educations = JsonConvert.DeserializeObject<List<DTO.Education>>(educationsJson);
                var languages = JsonConvert.DeserializeObject<List<DTO.Language>>(languagesJson);

                var fileDto = new FileCVDto
                {
                    FileName = Request.Form["FileName"],
                    Template = Request.Form["Template"],
                    FirstName = Request.Form["FirstName"],
                    LastName = Request.Form["LastName"],
                    Role = Request.Form["Role"],
                    Phone = Request.Form["Phone"],
                    Email = Request.Form["Email"],
                    Summary = Request.Form["Summary"],
                    WorkExperiences = JsonConvert.DeserializeObject<List<DTO.WorkExperience>>(Request.Form["WorkExperiences"]),
                    Educations = JsonConvert.DeserializeObject<List<DTO.Education>>(Request.Form["Educations"]),
                    Languages = JsonConvert.DeserializeObject<List<DTO.Language>>(Request.Form["Languages"]),
                    Skills = JsonConvert.DeserializeObject<List<string>>(Request.Form["Skills"])
                };
                await _fileUploadService.UploadFileAsync(file, userId, fileDto);

                return Ok(new { message = "File uploaded successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during upload: {ex.Message}");
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
        public async Task<IActionResult> UpdateFileCV([FromRoute] int id, IFormFile file, [FromForm] FileCVDto fileCVDto)
        {
            var userId = GetUserIdFromContext().ToString();
            var result = await _fileUploadService.UpdateFileCVAsync(file, id, userId, fileCVDto);

            if (result == null)
                return NotFound("Resume not found.");
            return Ok(result);
        }
        //לעדכון
        [Authorize]
        [HttpGet("fileCV/{id}")]
        public async Task<IActionResult> GetFileCV([FromRoute] int id)
        {
            try
            {
                var userId = GetUserIdFromContext().ToString();
                Console.WriteLine("=======================================77777777777777============================");
                Console.WriteLine("userId from token: " + userId);
                Console.WriteLine("=========================================7777777777================================");
                if (string.IsNullOrEmpty(userId) || userId == Guid.Empty.ToString())
                    return BadRequest("User ID is missing or invalid");

                var file = await _fileUploadService.GetFileCVByIdAsync(id, userId);

                if (file == null)
                    return NotFound();

                return Ok(file);
            }
            catch (Exception ex)
            {
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

            //return Ok(new { exists });
            return BadRequest(new { message = "ניתן לשמור עד 5 קורות חיים בלבד למשתמש." });
        }
    }
}
