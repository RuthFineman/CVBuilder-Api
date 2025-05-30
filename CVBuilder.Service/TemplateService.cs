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
        public async Task<string> AddTemplateAsync(IFormFile file, string fileName)
        {
            var fileTransferUtility = new TransferUtility(_s3Client);
            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = file.OpenReadStream(),  
                Key = $"cv-example/{fileName}",  
                BucketName = _bucketName,
                ContentType = file.ContentType, 
            };

            await fileTransferUtility.UploadAsync(uploadRequest);

            var template = new Template
            {
                Name = fileName,
                TemplateUrl = $"https://{_bucketName}.s3.amazonaws.com/cv-example/{fileName}",
            };

            await _templateRepository.AddTemplateAsync(template);

            return template.TemplateUrl;
        }
        public async Task<bool> DeleteTemplateAsync(string fileName)
        {
            try
            {
                var isFileDeleted = await _s3Client.DeleteObjectAsync(_bucketName, $"cv-example/{fileName}");
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
                throw new ApplicationException("Error deleting file", ex);
            }
        }
        private async Task<List<Template>> GetAllTemplatesFromS3Async()
        {
            var request = new ListObjectsV2Request
            {
                BucketName = _bucketName,
                Prefix = "cv-example/"
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
                    Id = 0, 
                    Name = Path.GetFileNameWithoutExtension(s3Object.Key),
                    TemplateUrl = fileUrl,
                    InUse = false 
                });
            }

            return templates;
        }
        public async Task<List<Template>> GetAllTemplatesCombinedAsync()
        {
            var s3Templates = await GetAllTemplatesFromS3Async(); 
            var dbTemplates = await _templateRepository.GetAllTemplatesFromDbAsync();

            foreach (var s3Template in s3Templates)
            {
                var dbMatch = dbTemplates.FirstOrDefault(t => t.TemplateUrl == s3Template.TemplateUrl);
                if (dbMatch != null)
                {
                    s3Template.Id = dbMatch.Id;
                    s3Template.InUse = dbMatch.InUse; 
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
                    return null;
                }

                if (!fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                {
                    fileName += ".png";
                }

                var url = $"https://cvfilebuilder.s3.eu-north-1.amazonaws.com/exampleCV/{fileName}";

                return url;
            }
            catch (Exception ex)
            {
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
