using CVBuilder.Core.DTOs;
using CVBuilder.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Swashbuckle.AspNetCore.Annotations;
using DTO = CVBuilder.Core.DTOs;
using Newtonsoft.Json;

namespace CVBuilder.Api.Controllers
{
    [ApiController]
    [Route("file-cv")]
    [Authorize]
    public class FileCVController : ControllerBase
    {
        private readonly IFileCVService _fileCVService;
        public FileCVController(IFileCVService fileCVService)
        {
            _fileCVService = fileCVService;
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
        [HttpGet("user-files")]
        public async Task<IActionResult> GetUserFiles()
        {
            var userId = GetUserIdFromContext().ToString();
            try
            {
                var files = await _fileCVService.GetUserFilesAsync(userId);
                return Ok(files);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "שגיאה בטעינת הקבצים: " + ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> SaveCVAndDetails(IFormFile file)
        {
            var userId = GetUserIdFromContext().ToString();
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }
            try
            {
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
                await _fileCVService.UploadFileAsync(file, userId, fileDto);
                return Ok(new { message = "File uploaded successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error uploading file: {ex.Message}");
            }
        }
        [HttpPut("update/{id}")]
        [SwaggerOperation(Summary = "Update resume with file upload", Description = "Allows updating a resume with the file and other details.")]
        public async Task<IActionResult> UpdateFileCV([FromRoute] int id, IFormFile file, [FromForm] FileCVDto fileCVDto)
        {
            Console.WriteLine("**********************************************************");
            Console.WriteLine(fileCVDto.Languages[0].Level);
            Console.WriteLine("**********************************************************");
            var userId = GetUserIdFromContext().ToString();
            var result = await _fileCVService.UpdateFileCVAsync(file, id, userId, fileCVDto);
            if (result == null)
                return NotFound("Resume not found.");
            return Ok(result);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteFile(int id)
        {
            var userId = GetUserIdFromContext().ToString();
            var result = await _fileCVService.DeleteFileByUserIdAsync(id, userId);
            if (result)
            {
                return Ok(new { message = "File deleted successfully." });
            }
            else
            {
                return Unauthorized(new { message = "File not found or doesn't belong to the user." });
            }
        }
        [HttpGet("fileCV/{id}")]
        public async Task<IActionResult> GetFileCV([FromRoute] int id)
        {
            try
            {
                var userId = GetUserIdFromContext().ToString();
                if (string.IsNullOrEmpty(userId) || userId == Guid.Empty.ToString())
                    return BadRequest("User ID is missing or invalid");
                var file = await _fileCVService.GetFileCVByIdAsync(id, userId);

                if (file == null)
                    return NotFound();

                return Ok(file);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}