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
    public interface IFileCVService
    {
        Task UploadFileAsync(IFormFile file, string userId, FileCVDto fileDto);
        Task<bool> DeleteFileByUserIdAsync(int fileId, string userId);
        Task<List<object>> GetUserFilesAsync(string userId);
        Task<FileCV> UpdateFileCVAsync(IFormFile newFile, int id, string userId, FileCVDto fileCVDto);
        Task<FileCV> GetFileCVByIdAsync(int id, string userId);
        //bool DoesFileExist(string key);
        //Task<int> GetFileCountByUserIdAsync(string userId);
    }
}
