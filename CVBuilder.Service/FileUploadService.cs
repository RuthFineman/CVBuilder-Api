using Amazon.S3.Model;
using Amazon.S3;
using CVBuilder.Core.Services;
using Microsoft.AspNetCore.Http;
using CVBuilder.Core.DTOs;
using CVBuilder.Core.Models;
using CVBuilder.Core.Repositories;

public class FileUploadService : IFileUploadService
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName = "cvfilebuilder";
    private readonly IFileRepository _fileRepository;  // נדרש לשמור את המידע על הקובץ ב-DB

    public FileUploadService(IAmazonS3 s3Client, IFileRepository fileRepository)
    {
        _s3Client = s3Client;
        _fileRepository = fileRepository;
    }

    private FileCV ConvertToFileCV(FileCVDto fileDto, string userId, IFormFile file)
    {
        return new FileCV
        {
            UserId = int.Parse(userId),
            FileName = file.FileName,
            FileUrl = $"https://{_bucketName}.s3.amazonaws.com/{userId}/{file.FileName}",
            UploadedAt = DateTime.UtcNow,
            FirstName = fileDto.FirstName,
            LastName = fileDto.LastName,
            Role = fileDto.Role,
            Phone = fileDto.Phone,
            Email = fileDto.Email,
            Summary = fileDto.Summary,
            WorkExperiences = fileDto.WorkExperiences?.Select(we => new CVBuilder.Core.Models.WorkExperience
            {
                Company = we.Company,
                Position = we.Position,
                StartDate = we.StartDate,
                EndDate = we.EndDate,
                Description = we.Description
            }).ToList() ?? new List<CVBuilder.Core.Models.WorkExperience>(),
            Languages = fileDto.Languages?.Select(l => new CVBuilder.Core.Models.Language
            {
                LanguageName = l.LanguageName,
                Proficiency = l.Proficiency
            }).ToList() ?? new List<CVBuilder.Core.Models.Language>(),

            Educations = fileDto.Educations?.Select(e => new CVBuilder.Core.Models.Education
            {
                Institution = e.Institution,
                Degree = e.Degree
            }).ToList() ?? new List<CVBuilder.Core.Models.Education>(),

            Skills = fileDto.Skills ?? new List<string>()
        };
    }

    public async Task UploadFileAsync(IFormFile file, string userId, FileCVDto fileDto)
    {
        if (string.IsNullOrEmpty(userId))
        {
            //Console.WriteLine("UserId is required");
            throw new ArgumentException("UserId is required");
        }

        if (file == null || file.Length == 0)
        {
            //Console.WriteLine("No file provided or file is empty");
            throw new InvalidOperationException("No file provided or file is empty");
        }

        if (fileDto == null)
        {
            //Console.WriteLine("FileCVDto cannot be null");
            throw new ArgumentException("FileCVDto cannot be null");
        }

        try
        {
            // העלאת הקובץ ל-AWS S3
            using (var stream = file.OpenReadStream())
            {
                if (stream.Length == 0)
                {
                    //Console.WriteLine("The file is empty.");
                    throw new InvalidOperationException("The file is empty.");
                }

                var uploadRequest = new PutObjectRequest
                {
                    BucketName = _bucketName,
                    Key = $"{userId}/{file.FileName}", // ודא שאין בעיה בשמות הקבצים כאן
                    InputStream = stream,
                    ContentType = file.ContentType
                };

                var response = await _s3Client.PutObjectAsync(uploadRequest);
                //Console.WriteLine($"File uploaded to S3: {response.HttpStatusCode}");

                // המרה מ-FileCVDto ל-FileCV
                var fileRecord = ConvertToFileCV(fileDto, userId, file);

                // בדוק שהמידע המתקבל ממיר כראוי
                if (fileRecord == null)
                {
                    Console.WriteLine("Error: Failed to convert FileCVDto to FileCV.");
                    throw new InvalidOperationException("Failed to convert FileCVDto to FileCV.");
                }
                Console.WriteLine($"Email: {fileRecord.Email}");

                if (string.IsNullOrEmpty(fileRecord.Email))
                {
                    throw new ArgumentException("Email is required.");
                }
                // שמירת הנתונים בבסיס הנתונים
           

                await _fileRepository.SaveFileRecordAsync(fileRecord);

                Console.WriteLine("File record saved in database");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error uploading file: {ex.Message}");
            throw new InvalidOperationException("An error occurred while uploading the file.", ex);
        }
    }
    public async Task<bool> DeleteFileByUserIdAsync(int fileId, int userId)
    {
        var file = await _fileRepository.GetFileByUserIdAsync(fileId, userId);
        if (file == null) return false;

        // שלב 1: מחיקה מ-AWS S3
        if (!string.IsNullOrEmpty(file.FileName))
        {
            Console.WriteLine(userId + "/" + file.FileName);
            Console.WriteLine("FileName: " + file.FileName);
            Console.WriteLine("FileName: " + file.FileName);
            await _s3Client.DeleteObjectAsync(_bucketName, userId+"/"+file.FileName); 
            await _s3Client.DeleteObjectAsync(_bucketName," 115 / קורות_חיים_mmmmm_mmmm_4980.pdf");
        }

        // שלב 2: מחיקה מה-Database
        await _fileRepository.DeleteFileCVAsync(file.Id);
        return true;
    }

}
