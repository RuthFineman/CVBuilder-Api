using Amazon.S3.Model;
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
    public class FileRepository : IFileCVRepository
    {
        private readonly CVBuilderDbContext _context;

        public FileRepository(CVBuilderDbContext context)
        {
            _context = context;
        }
        public async Task<List<FileCV>> GetFilesByUserIdAsync(string userId)
        {
            return await _context.FileCVs
                .Where(f => f.UserId.ToString() == userId)
                .ToListAsync();
        }

        public async Task SaveFileRecordAsync(FileCV fileRecord)
        {
            await _context.FileCVs.AddAsync(fileRecord);
            Console.WriteLine("Template value::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::: ");
            Console.WriteLine("Template value: " + fileRecord.Template);
            Console.WriteLine("Template value::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::: ");

            await _context.SaveChangesAsync(); // וזה שומר הכל יחד
        }

        public async Task DeleteFileCVAsync(int fileId)
        {
            var file = await _context.FileCVs.FindAsync(fileId);
            if (file != null)
            {
                _context.FileCVs.Remove(file); // מסיר את הקובץ מה-DB
                await _context.SaveChangesAsync(); // שומר את השינויים
            }
        }
       
        public async Task<FileCV> GetFileByUserIdAsync(int fileId, string userId)
        {
              return await _context.FileCVs
    .Include(f => f.WorkExperiences)
    .Include(f => f.Educations)
    .Include(f => f.Languages)
    .Where(f => f.Id == fileId && f.UserId.ToString() == userId)
    .FirstOrDefaultAsync();
        }
        public async Task UpdateAsync(FileCV updatedFile)
        {
            _context.FileCVs.Update(updatedFile);
            await _context.SaveChangesAsync();
        }
        public async Task<int> GetFileCountByUserIdAsync(string userId)
        {
            return await _context.FileCVs
                .Where(f => f.UserId.ToString() == userId)
                .CountAsync();
        }

    }
}
