using CVBuilder.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVBuilder.Core.Repositories
{
    public interface IFileRepository
    {
        Task SaveFileRecordAsync(FileCV fileRecord);
        Task DeleteFileCVAsync(int fileId);
        Task<FileCV> GetFileByUserIdAsync(int fileId, int userId);
    }
}
