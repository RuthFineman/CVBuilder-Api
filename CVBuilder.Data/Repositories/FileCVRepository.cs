using Amazon.S3;
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
        private readonly IAmazonS3 _s3Client;
        public FileCVRepository(CVBuilderDbContext context, IAmazonS3 s3Client)
        {
            _context = context;
            _s3Client = s3Client;

        }
        //להחזרת כל הקורות חיים
        public async Task<List<string>> FetchFilesByUserIdAsync(string userId)
        {
            var bucketName = "cvfilebuilder";
            var request = new ListObjectsV2Request
            {
                BucketName = bucketName,
                Prefix = $"{userId}/"
            };

            var response = await _s3Client.ListObjectsV2Async(request);

            // הוסף לוג כדי לבדוק את התגובה
            Console.WriteLine($"Number of files found: {response.S3Objects.Count}");

            return response.S3Objects.Select(o => o.Key).ToList();
        }

        //delete
        public async Task DeleteFileCVAsync(int fileId)
        {
            var file = await _context.FileCVs.FindAsync(fileId);
            if (file != null)
            {
                _context.FileCVs.Remove(file);
                await _context.SaveChangesAsync();
            }
        }

        //create

        public async Task<List<FileCV>> GetByUserIdAsync(int userId)
        {
            Console.WriteLine($"[DEBUG] userId: {userId}");
            return await _context.FileCVs.Where(f => f.UserId == userId).ToListAsync();
        }
        //create
        public async Task AddAsync(FileCV fileCV)
        {
            await _context.FileCVs.AddAsync(fileCV);
            await _context.SaveChangesAsync();
        }

        //update getfilebyuserid delete
        public async Task<FileCV> GetFileByUserIdAsync(int fileId, int userId)
        {
            return await _context.FileCVs
                .FirstOrDefaultAsync(f => f.Id == fileId && f.UserId == userId);
        }

        //update

        //public async Task UpdateAsync(FileCV fileCV)
        //{
        //    var existingFile = await _context.FileCVs.FindAsync(fileCV.Id);
        //    if (existingFile != null)
        //    {

        //        existingFile.Name = fileCV.Name ?? existingFile.Name;
        //        existingFile.Id = fileCV.Id ;
        //        existingFile.FirstName = fileCV.FirstName ?? existingFile.FirstName;
        //        existingFile.LastName = fileCV.LastName ?? existingFile.LastName;
        //        existingFile.Email = fileCV.Email ?? existingFile.Email;
        //        existingFile.Phone = fileCV.Phone ?? existingFile.Phone;
        //        existingFile.Summary = fileCV.Summary ?? existingFile.Summary;
        //        existingFile.Skills = fileCV.Skills ?? existingFile.Skills;
        //        existingFile.Languages = fileCV.Languages ?? existingFile.Languages;
        //        await _context.SaveChangesAsync();
        //    }
        //}
        public async Task UpdateAsync(FileCV fileCV)
        {
            var existingFile = await _context.FileCVs.FindAsync(fileCV.Id);
            if (existingFile != null)
            {
                //existingFile.FirstName = fileCV.FirstName ?? existingFile.FirstName;
                //existingFile.LastName = fileCV.LastName ?? existingFile.LastName;
                //existingFile.Email = fileCV.Email ?? existingFile.Email;
                //existingFile.Phone = fileCV.Phone ?? existingFile.Phone;
                //existingFile.Summary = fileCV.Summary ?? existingFile.Summary;
                //existingFile.Skills = fileCV.Skills ?? existingFile.Skills;

                //// עדכון חוויות עבודה וחינוך
                //existingFile.WorkExperiences = fileCV.WorkExperiences ?? existingFile.WorkExperiences;
                //existingFile.Educations = fileCV.Educations ?? existingFile.Educations;

                await _context.SaveChangesAsync();
            }
        }


    }
}

