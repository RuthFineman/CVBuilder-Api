using CVBuilder.Core.DTOs;
using CVBuilder.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVBuilder.Core.Repositories
{
    public interface IFileCVRepository
    {
        Task SaveFileRecordAsync(FileCV fileRecord);
        Task DeleteFileCVAsync(int fileId);
        Task<FileCV> GetFileByUserIdAsync(int fileId, string userId);
        Task<List<FileCV>> GetFilesByUserIdAsync(string userId);
        Task UpdateAsync(FileCV fileCV);
        Task<List<FileCVStatsDto>> GetUploadStatsAsync();
    }
}
