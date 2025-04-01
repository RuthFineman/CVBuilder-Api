using Amazon.S3.Transfer;
using Amazon.S3;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using CVBuilder.Core.Services;
using Microsoft.Extensions.Configuration;

namespace CVBuilder.Service
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly IConfiguration _configuration;

        public FileUploadService(IAmazonS3 s3Client, IConfiguration configuration)
        {
            _s3Client = s3Client;
            _configuration = configuration;
        }

        public async Task UploadFileAsync(IFormFile file)
        {
            // טען את ה-BucketName מתוך משתני הסביבה
            string bucketName = _configuration["AWS_S3_BUCKET_NAME"];

            if (string.IsNullOrEmpty(bucketName))
            {
                throw new Exception("Bucket name is not configured.");
            }

            Console.WriteLine($"Bucket Name: {bucketName}"); // לוג של שם ה-bucket

            try
            {
                if (file.Length > 0)
                {
                    var fileTransferUtility = new TransferUtility(_s3Client);

                    using (var stream = file.OpenReadStream())
                    {
                        await fileTransferUtility.UploadAsync(stream, bucketName, file.FileName);
                    }
                }
            }
            catch (AmazonS3Exception s3Ex)
            {
                throw new Exception($"שגיאה בהעלאת הקובץ ל-S3: {s3Ex.Message}", s3Ex);
            }
            catch (Exception ex)
            {
                throw new Exception("שגיאה בהעלאת הקובץ ל-S3", ex);
            }
        }

    }
}
