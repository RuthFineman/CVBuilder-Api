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
    public class FileCVRepository : IFileCVRepository
    {
        private readonly CVBuilderDbContext _context;

        public FileCVRepository(CVBuilderDbContext context)
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
        public async Task<string> GetFileNameByIndexAsync(int index)
        {
            try
            {
                var template = await _context.Templates.FindAsync(index);
                if (template == null)
                {
                    return null;
                }
                return template.Name;
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching file name from DB", ex);
            }
        }
        public async Task<List<FileCVStatsDto>> GetUploadStatsAsync()
        {
            return await _context.FileCVs
                .GroupBy(r => r.UploadedAt.Hour)
                .Select(g => new FileCVStatsDto
                {
                    Time = DateTime.Today.AddHours(g.Key),
                    Count = g.Count()
                })
                .OrderBy(g => g.Time)
                .ToListAsync();
        }
    }
}
