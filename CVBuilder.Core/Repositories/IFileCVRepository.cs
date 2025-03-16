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
        Task<List<FileCV>> GetFilesByUserIdAsync(int userId);
        Task DeleteFileCVAsync(int fileId);
        Task<List<FileCV>> GetByUserIdAsync(int userId);
        Task AddAsync(FileCV fileCV);
        Task UpdateAsync(FileCV fileCV);
        Task<FileCV> GetFileByUserIdAsync(int fileId, int userId);
    }
}
