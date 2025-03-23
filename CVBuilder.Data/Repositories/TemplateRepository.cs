using CVBuilder.Core.Models;
using CVBuilder.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
namespace CVBuilder.Data.Repositories
{
    public class TemplateRepository : ITemplateRepository
    {
        private readonly CVBuilderDbContext _context;
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName = "cvfilebuilder";

        public TemplateRepository(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;


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
                {
                    continue;
                }
                string fileUrl = $"https://{_bucketName}.s3.amazonaws.com/{s3Object.Key}";
                fileUrls.Add(fileUrl);
            }
            return fileUrls;
        }
        public async Task<string> GetFileByIndexAsync(int index)
        {
            var request = new ListObjectsV2Request
            {
                BucketName = _bucketName,
                Prefix = "CVFilebuilder/"
            };

            var response = await _s3Client.ListObjectsV2Async(request);

            var fileList = response.S3Objects
                .Where(obj => !obj.Key.EndsWith("/") && obj.Size > 0)
                .Select(obj => obj.Key)
                .OrderBy(name => name) // למקרה שהשמות לא בסדר מסודר
                .ToList();

            if (index < 0 || index >= fileList.Count)
                return null;

            string fileName = fileList[index ];

            return $"https://{_bucketName}.s3.amazonaws.com/{fileName}";
        }
        //public async Task<string?> GetFirstFileAsync()
        //{
        //    var request = new ListObjectsV2Request
        //    {
        //        BucketName = _bucketName,
        //        MaxKeys = 1
        //    };

        //    var response = await _s3Client.ListObjectsV2Async(request);
        //    return response.S3Objects.FirstOrDefault()?.Key;
        //}

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
