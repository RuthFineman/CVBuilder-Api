﻿using Amazon.S3.Model;
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
        public async Task<List<FileCV>> GetFilesByUserIdAsync(string userId)
        {
            return await _context.FileCVs
                .Where(f => f.UserId.ToString() == userId)
                .ToListAsync();
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
                _context.FileCVs.Remove(file); // מסיר את הקובץ מה-DB
                await _context.SaveChangesAsync(); // שומר את השינויים
            }
        }

        //  והעדכון בשביל המחיקה
        public async Task<FileCV> GetFileByUserIdAsync(int fileId, string userId)
        {
            return await _context.FileCVs
                .Where(f => f.Id == fileId && f.UserId.ToString() == userId) // המרה של UserId מ- int ל-string להשוואה
                .FirstOrDefaultAsync(); // מחזיר את הקובץ אם נמצא או null אם לא נמצא
        }

        //public async Task<FileCV> GetFileByUrlAsync(string fileUrl)
        //{
        //    return await _context.FileCVs
        //        .Where(f => f.FileUrl == fileUrl)
        //        .FirstOrDefaultAsync();
        //}

        //לעדכון
        public async Task UpdateAsync(FileCV fileCV)
        {
            var existingFile = await _context.FileCVs.FindAsync(fileCV.Id);
            if (existingFile != null)
            {
                await _context.SaveChangesAsync();
            }
        }
    }
}
