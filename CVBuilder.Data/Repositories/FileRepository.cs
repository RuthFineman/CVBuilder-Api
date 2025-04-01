using CVBuilder.Core.DTOs;
using CVBuilder.Core.Models;
using CVBuilder.Core.Repositories;
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

        public async Task SaveFileRecordAsync(string fileName, string fileUrl)
        {
            var fileRecord = new FileCV
            {
                FileName = fileName,
                FileUrl = fileUrl,
                UploadedAt = DateTime.UtcNow
            };

            _context.FileCVs.Add(fileRecord);
            await _context.SaveChangesAsync();
        }
    }
}
