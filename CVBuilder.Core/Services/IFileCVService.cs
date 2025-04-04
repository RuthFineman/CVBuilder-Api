using CVBuilder.Core.DTOs;
using CVBuilder.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVBuilder.Core.Services
{
    public interface IFileCVService
    {
        //Task<List<FileCVDto>> GetFilesByUserIdAsync(int userId);
        Task<List<string>> GetUserFilesAsync(string userId);
        Task<bool> DeleteFileByUserIdAsync(int fileId, int userId);
        //Task<FileCVDto> CreateFileCVAsync(FileCVDto fileCVDto, int userId);
        Task<FileCVDto> GetFileByUserIdAsync(int fileId, int userId);
        Task<FileCV> UpdateFileCVAsync(int id, int userId, FileCVDto fileCVDto);
    }
}
