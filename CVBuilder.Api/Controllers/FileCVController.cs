using CVBuilder.Core.DTOs;
using CVBuilder.Core.Services;
using CVBuilder.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CVBuilder.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileCVController : ControllerBase
    {
        private readonly IFileCVService _fileCVService;

        public FileCVController(IFileCVService fileCVService)
        {
            _fileCVService = fileCVService;
        }
        [Authorize] 
        [HttpGet("user-files")]
        public async Task<IActionResult> GetUserFiles()
        {
            var userId = GetUserIdFromContext();
            var files = await _fileCVService.GetFilesByUserIdAsync(userId);
            return Ok(files);
        }

        [Authorize]
        [HttpDelete("remove/{id}")]
        public async Task<IActionResult> DeleteFile(int id)
        {
            var userId = GetUserIdFromContext(); 
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


        [Authorize]
        [HttpPost("add")]
        public async Task<IActionResult> CreateFileCV([FromBody] FileCVDto fileCVDto)
        {
            var userId = GetUserIdFromContext();
            try
            {
                var newFileCV = await _fileCVService.CreateFileCVAsync(fileCVDto, userId);
                return Ok(new FileCVDto
                {
                    Name = newFileCV.Name,
                    FirstName = newFileCV.FirstName,
                    LastName = newFileCV.LastName,
                    Email = newFileCV.Email,
                    Phone = newFileCV.Phone,
                    Summary = newFileCV.Summary,
                    Skills = newFileCV.Skills ?? new List<string>(),
                    Languages = newFileCV.Languages ?? new List<string>()
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("modify/{id}")]
        public async Task<IActionResult> UpdateFileCV(int id, [FromBody] FileCVDto fileCVDto)
        {
            var userId = GetUserIdFromContext(); 
            var result = await _fileCVService.UpdateFileCVAsync(id, userId, fileCVDto);

            if (result == null)
                return NotFound("Resume not found.");

            return Ok(result);
        }

        [HttpGet("user-files/{fileId}")]
        public async Task<IActionResult> GetFileByUserId(int fileId)
        {
            var userId = GetUserIdFromContext();
            var result = await _fileCVService.GetFileByUserIdAsync(fileId, userId);
            if (result == null)
            {
                return NotFound("No resume found.");
            }
            return Ok(result);
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

    }
}
