using CVBuilder.Core.DTOs;
using CVBuilder.Core.Models;
using CVBuilder.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVBuilder.Data.Repositories
{
    public class FileRepository: IFileRepository
    {
        private readonly CVBuilderDbContext _context;

        public FileRepository(CVBuilderDbContext context)
        {
            _context = context;
        }
        public async Task SaveFileRecordAsync(FileCV fileRecord)
        {
            _context.FileCVs.Add(fileRecord);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteFileCVAsync(int fileId)
        {
            var file = await _context.FileCVs.FindAsync(fileId);
            if (file != null)
            {
                _context.FileCVs.Remove(file);
                await _context.SaveChangesAsync();
            }
        }
        //בשביל המחיקה
        public async Task<FileCV> GetFileByUserIdAsync(int fileId, int userId)
        {
            return await _context.FileCVs
                .Where(f => f.Id == fileId && f.UserId == userId)
                .FirstOrDefaultAsync();
        }

    }
}
