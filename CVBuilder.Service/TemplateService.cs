using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using CVBuilder.Core.Models;
using CVBuilder.Core.Repositories;
using CVBuilder.Core.Services;
using Microsoft.AspNetCore.Http;

namespace CVBuilder.Service
{
    public class TemplateService : ITemplateService
    {
        private readonly ITemplateRepository _templateRepository;
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName = "cvfilebuilder";

        public TemplateService(ITemplateRepository ITemplateRepository, IAmazonS3 s3Client)
        {
            _templateRepository = ITemplateRepository;
            _s3Client = s3Client;
        }
        //הוספת תבנית
        public async Task<string> AddTemplateAsync(IFormFile file, string fileName)
        {
            // יצירת בקשה להעלאת הקובץ ל-S3
            var fileTransferUtility = new TransferUtility(_s3Client);
            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = file.OpenReadStream(),  // שימוש ב-InputStream של ה-File
                Key = $"exampleCV/{fileName}",  // כאן אנחנו מוסיפים את התיקיה exampleCV/
                BucketName = _bucketName,  // שם הבקט שלך
                ContentType = file.ContentType,  // סוג הקובץ המתאים
            };

            // העלאת הקובץ ל-S3
            await fileTransferUtility.UploadAsync(uploadRequest);

            // יצירת פרטי התבנית להוספה למסד הנתונים
            var template = new Template
            {
                Name = fileName,
                // ה-URL מעודכן, כולל התיקיה exampleCV/
                TemplateUrl = $"https://{_bucketName}.s3.amazonaws.com/exampleCV/{fileName}",
            };

            // שמירת פרטי התבנית במסד הנתונים
            await _templateRepository.AddTemplateAsync(template);

            return template.TemplateUrl;
        }
        //מחיקת תבנית
        public async Task<bool> DeleteTemplateAsync(string fileName)
        {
            try
            {
                // קריאה למחיקת הקובץ ב-S3
                var isFileDeleted = await _s3Client.DeleteObjectAsync(_bucketName, $"exampleCV/{fileName}");
                if (isFileDeleted.HttpStatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    await _templateRepository.DeleteTemplateAsync(fileName);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                // טיפול בשגיאות
                throw new ApplicationException("Error deleting file", ex);
            }
        }
        private async Task<List<Template>> GetAllTemplatesFromS3Async()
        {
            var request = new ListObjectsV2Request
            {
                BucketName = _bucketName,
                Prefix = "exampleCV/"
            };

            var response = await _s3Client.ListObjectsV2Async(request);
            var templates = new List<Template>();

            foreach (var s3Object in response.S3Objects)
            {
                if (s3Object.Key.EndsWith("/") || s3Object.Size == 0)
                    continue;

                string fileUrl = $"https://{_bucketName}.s3.amazonaws.com/{s3Object.Key}";

                templates.Add(new Template
                {
                    Id = 0, // כי אין ID ב-S3
                    Name = Path.GetFileNameWithoutExtension(s3Object.Key),
                    TemplateUrl = fileUrl,
                    InUse = false // ברירת מחדל, אלא אם יש לך דרך לדעת
                });
            }

            return templates;
        }
        public async Task<List<Template>> GetAllTemplatesCombinedAsync()
        {
            var s3Templates = await GetAllTemplatesFromS3Async(); // שומר על הפונקציה כפי שהיא
            var dbTemplates = await _templateRepository.GetAllTemplatesFromDbAsync();

            foreach (var s3Template in s3Templates)
            {
                var dbMatch = dbTemplates.FirstOrDefault(t => t.TemplateUrl == s3Template.TemplateUrl);
                if (dbMatch != null)
                {
                    s3Template.Id = dbMatch.Id;
                    s3Template.InUse = dbMatch.InUse; // כאן אנחנו מעדכנים את הערך לפי DB
                }
            }

            return s3Templates;
        }
        public async Task<string> GetFileAsync(int index)
        {
            try
            {
                var fileName = await _templateRepository.GetFileNameByIndexAsync(index);

                if (string.IsNullOrEmpty(fileName))
                {
                    // לוג - לא נמצא קובץ עם האינדקס הזה
                    Console.WriteLine($"GetFileAsync: No file found for index {index}");
                    return null;
                }

                // בדיקה אם הסיומת כבר קיימת
                if (!fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                {
                    fileName += ".png";
                }

                var url = $"https://cvfilebuilder.s3.eu-north-1.amazonaws.com/exampleCV/{fileName}";
                //Console.WriteLine($"GetFileAsync: Generated URL: {url}");

                return url;
            }
            catch (Exception ex)
            {
                // לוג - הדפס את השגיאה המלאה
                Console.WriteLine($"GetFileAsync: Exception: {ex.Message}\n{ex.StackTrace}");
                throw new Exception("Error creating file URL", ex);
            }
        }

        public async Task<Template?> UpdateTemplateStatusAsync(int id, bool newStatus)
        {
            var template = await _templateRepository.GetByIdAsync(id);
            if (template == null)
                return null;

            template.InUse = newStatus;
            await _templateRepository.UpdateAsync(template);

            return template;
        }
    }
}
