using CVBuilder.Core.DTOs;
using CVBuilder.Core.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVBuilder.Core.Services
{
    public interface IFileUploadService
    {
        Task UploadFileAsync(IFormFile file, string userId, FileCVDto fileDto);
        Task<bool> DeleteFileByUserIdAsync(int fileId, string userId);
        Task<List<object>> GetUserFilesAsync(string userId);
        Task<FileCV> GetFileByUrlAsync(string fileUrl);
        Task<FileCV> UpdateFileCVAsync(IFormFile newFile, int id, string userId, FileCVDto fileCVDto);
    }
}
