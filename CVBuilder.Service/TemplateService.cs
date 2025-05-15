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
    public class TemplateService: ITemplateService
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

        public async Task<List<string>> GetAllTamplatesAsync()
        {
            var request = new ListObjectsV2Request
            {
                BucketName = _bucketName,
                Prefix = "exampleCV/"
            };

            var response = await _s3Client.ListObjectsV2Async(request);
            var fileUrls = new List<string>();

            foreach (var s3Object in response.S3Objects)
            {
                if (s3Object.Key.EndsWith("/") || s3Object.Size == 0)
                    continue;

                string fileUrl = $"https://{_bucketName}.s3.amazonaws.com/{s3Object.Key}";
                fileUrls.Add(fileUrl);
            }

            return fileUrls;
        }
        //שליפה של קובץ בודד
        //public async Task<string?> GetFileAsync(int templateId)
        //{
        //    // שלב 1: שלוף מהמסד
        //    var fileName = await _templateRepository.GetFileNameByIdAsync(templateId);
        //    if (string.IsNullOrEmpty(fileName))
        //        return null;

        //    // שלב 2: בדוק אם קיים ב-AWS
        //    var fileUrl = await _templateRepository.GetS3FileUrlIfExistsAsync(fileName);
        //    return fileUrl;
        //}
        //קבלת תבנית אחת
        public async Task<string> GetFileAsync(int index)
        {
            // קבל את שם הקובץ מה-DB
            var fileName = await _templateRepository.GetFileNameByIndexAsync(index);
            if (string.IsNullOrEmpty(fileName))
                return null;

            // צור URL זמני להורדה מ-AWS S3
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = $"CVFilebuilder/{fileName}",
                Expires = DateTime.UtcNow.AddMinutes(30)
            };

            var url = _s3Client.GetPreSignedURL(request);
            return url;
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
