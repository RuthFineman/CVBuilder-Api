﻿using CVBuilder.Core.Models;
using CVBuilder.Core.Repositories;
using Amazon.S3.Model;
using Microsoft.EntityFrameworkCore;
namespace CVBuilder.Data.Repositories
{
    public class TemplateRepository : ITemplateRepository
    {
        private readonly CVBuilderDbContext _context;
        //private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName = "cvfilebuilder";

        public TemplateRepository(CVBuilderDbContext context)
        {
            _context = context;
        }
        //העלאת תבנית
        public async Task AddTemplateAsync(Template template)
        {
            _context.Templates.Add(template);
            await _context.SaveChangesAsync();
        }
        //מחיקת תבנית
        public async Task DeleteTemplateAsync(string fileName)
        {
            // חיפוש התבנית על פי שם הקובץ
            var template = await _context.Templates.FirstOrDefaultAsync(t => t.Name == fileName);

            if (template != null)
            {
                // מחיקת התבנית ממסד הנתונים
                _context.Templates.Remove(template);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException("Template not found.");
            }
        }
        //קבלת כל התבניות
        public async Task<List<Template>> GetAllTemplatesFromDbAsync()
        {
            return await _context.Templates
                .Where(t => !string.IsNullOrEmpty(t.TemplateUrl))
                .ToListAsync();
        }
        //קבלת תבנית אחת
        public async Task<string> GetFileNameByIndexAsync(int index)
        {
            var template = await _context.Templates
                .OrderBy(t => t.Id) // או כל סדר שתרצה
                .Skip(index)
                .Take(1)
                .FirstOrDefaultAsync();

            return template?.Name; // או כל שדה שמכיל את שם הקובץ ב-S3
        }
        public async Task<Template?> GetByIdAsync(int id)
        {
            return await _context.Templates.FindAsync(id);
        }

        public async Task UpdateAsync(Template template)
        {
            _context.Templates.Update(template);
            await _context.SaveChangesAsync();
        }
        public Template? GetByIdAndUserId(int id, int userId)
        {
            return _context.Templates.FirstOrDefault(t => t.Id == id);
        }

        public void Add(Template template)
        {
            _context.Templates.Add(template);
            _context.SaveChanges();
        }
        public bool Delete(int id)
        {
            var template = _context.Templates.Find(id);
            if (template == null)
                return false;

            _context.Templates.Remove(template);
            _context.SaveChanges();
            return true;
        }
    }
}

