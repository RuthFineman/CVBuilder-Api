using Amazon.S3.Model;
using Amazon.S3;
using CVBuilder.Core.Services;
using Microsoft.AspNetCore.Http;
using CVBuilder.Core.DTOs;
using CVBuilder.Core.Models;
using CVBuilder.Core.Repositories;
using Amazon.S3.Transfer;
using System.Net;

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
    public async Task<List<object>> GetUserFilesAsync(string userId)
    {
        // שליפת קבצים מה-DB
        var dbFiles = await _fileRepository.GetFilesByUserIdAsync(userId);

        // שליפת קבצים מ-S3
        var s3Files = await FetchFilesFromS3Async(userId);

        // יצירת אובייקטים מה-DB עם ID
        var dbFileDtos = dbFiles.Select(f => new {
            id = f.Id.ToString(),  // ID מה-DB
            path = (string)null    // לא נשלף Path מה-DB
        }).ToList(); // רשימה של קבצים מה-DB

        // יצירת אובייקטים מ-S3 עם Path
        var s3FileDtos = s3Files.Select(f => new {
            id = (string)null,     // לא נשלף ID מ-S3
            path = f              // Path מ-S3
        }).ToList(); // רשימה של קבצים מ-S3

        // חיבור בין רשימות: ה-ID מה-DB ו-Path מ-S3
        var result = dbFileDtos.Zip(s3FileDtos, (dbFile, s3File) => new {
            id = dbFile.id,         // ID מה-DB
            path = s3File.path     // Path מ-S3
        }).ToList();

        return result.Cast<object>().ToList();
    }

    private async Task<List<string>> FetchFilesFromS3Async(string userId)
    {
        var request = new ListObjectsV2Request
        {
            BucketName = _bucketName,
            Prefix = $"{userId}/"
        };

        var response = await _s3Client.ListObjectsV2Async(request);
        return response.S3Objects.Select(o => o.Key).ToList();
    }

    public async Task UploadFileAsync(IFormFile file, string userId, FileCVDto fileDto)
    {
        if (string.IsNullOrEmpty(userId))
        {
            throw new ArgumentException("UserId is required");
        }
        if (file == null || file.Length == 0)
        {
            throw new InvalidOperationException("No file provided or file is empty");
        }
        if (fileDto == null)
        {
            throw new ArgumentException("FileCVDto cannot be null");
        }
        try
        {
            // העלאת הקובץ ל-AWS S3
            using (var stream = file.OpenReadStream())
            {
                if (stream.Length == 0)
                {
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
                // המרה מ-FileCVDto ל-FileCV
                var fileRecord = ConvertToFileCV(fileDto, userId, file);

                if (fileRecord == null)
                {
                    throw new InvalidOperationException("Failed to convert FileCVDto to FileCV.");
                }
                if (string.IsNullOrEmpty(fileRecord.Email))
                {
                    throw new ArgumentException("Email is required.");
                }
                // שמירת הנתונים בבסיס הנתונים
                await _fileRepository.SaveFileRecordAsync(fileRecord);
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("An error occurred while uploading the file.", ex);
        }
    }
    public async Task<bool> DeleteFileByUserIdAsync(int fileId, string userId)
    {

        // מחפש את הקובץ לפי fileId ו-userId
        var file = await _fileRepository.GetFileByUserIdAsync(fileId, userId);
        if (file == null) return false; // אם לא נמצא קובץ כזה או שהקובץ לא שייך למשתמש
        // שלב 1: מחיקה מ-AWS S3
        if (!string.IsNullOrEmpty(file.FileName))
        {
            await _s3Client.DeleteObjectAsync(_bucketName, $"{userId}/{file.FileName}");
        }
        // שלב 2: מחיקה מה-Database
        await _fileRepository.DeleteFileCVAsync(file.Id);
        return true; // מחיקה הצליחה
    }
    public async Task<FileCV> UpdateFileCVAsync(IFormFile newFile, int id, string userId, FileCVDto fileCVDto)
    {
        var oldFile = await _fileRepository.GetFileByUserIdAsync(id, userId);
        if (oldFile == null) return null;

        // שלב 2: מחיקת הקובץ הישן מה-S3 לפי שם הקובץ הישן ששמור ב-DB
        var oldKey = $"{userId}/{oldFile.FileName}";
        try
        {
            // שלב 1: מחיקה מ-AWS S3
            if (!string.IsNullOrEmpty(fileCVDto.FileName))
            {
                await _s3Client.DeleteObjectAsync(_bucketName, oldKey);
                Console.WriteLine("deleteeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee");
      
            }
        }
        catch (AmazonS3Exception e)
        {
            // לא נמצא הקובץ - טיפלנו בשגיאת "NoSuchKey"
            if (e.ErrorCode != "NoSuchKey")
            {
                throw; // זרוק שגיאה אם הייתה בעיה אחרת
            }
            // במקרה שהקובץ לא נמצא - לא עשינו כלום
            Console.WriteLine("File doesn't exist, skipping delete.");
        }
        // העלאת הקובץ החדש
        using (var stream = new MemoryStream())
        {
            await newFile.CopyToAsync(stream);
            var putRequest = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = $"{userId}/{newFile.FileName}",  
                InputStream = stream,
                ContentType = newFile.ContentType
            };
            var response = await _s3Client.PutObjectAsync(putRequest);
            if (response.HttpStatusCode != HttpStatusCode.OK)
            {
                throw new Exception("Failed to upload file to S3");
            }
        }
        // עדכון פרטי הקובץ ב-db
        Console.WriteLine("===============================================");
        Console.WriteLine($"===> Looking for fileId: {id}, userId: {userId}");
        Console.WriteLine("===============================================");
        var file = await _fileRepository.GetFileByUserIdAsync(id, userId);
        if (file == null) return null; 
        file.Id = id;
        file.FirstName = fileCVDto.FirstName ?? file.FirstName;
        file.LastName = fileCVDto.LastName ?? file.LastName;
        file.FileName = newFile.FileName;
        file.Email = fileCVDto.Email ?? file.Email;
        file.Phone = fileCVDto.Phone ?? file.Phone;
        file.Summary = fileCVDto.Summary ?? file.Summary;
        if (fileCVDto.Skills != null)
            file.Skills = fileCVDto.Skills;

        //צריך לבדוק מה קורה האם לא מעדכנים את הכישורים, האם זה מתאפס או לא?
        //if (fileCVDto.Skills != null)
        //    file.Skills = fileCVDto.Skills;

        // המרת חוויות עבודה עם namespace מלא
        file.WorkExperiences = fileCVDto.WorkExperiences?.Select(we => new CVBuilder.Core.Models.WorkExperience
        {
            Company = we.Company,
            Position = we.Position,
            StartDate = we.StartDate,
            EndDate = we.EndDate,
            Description = we.Description
        }).ToList() ?? new List<CVBuilder.Core.Models.WorkExperience>();  // טיפול במצב שבו השדה ריק
        // המרת חינוך עם namespace מלא
        file.Educations = fileCVDto.Educations?.Select(e => new CVBuilder.Core.Models.Education
        {
            Institution = e.Institution,
            Degree = e.Degree
        }).ToList() ?? new List<CVBuilder.Core.Models.Education>();  // טיפול במצב שבו השדה ריק
        await _fileRepository.UpdateAsync(file);
        return file;
    }
    //לעדכון
    public async Task<FileCV> GetFileCVByIdAsync(int id, string userId)
    {
        Console.WriteLine($"Parsed userId = {int.Parse(userId)}");
        return await _fileRepository.GetFileByUserIdAsync(id, userId);
    }
    public bool DoesFileExist(string key)
    {
        try
        {
            var request = new GetObjectMetadataRequest
            {
                BucketName = _bucketName,
                Key = key
            };

            var response = _s3Client.GetObjectMetadataAsync(request).Result;
            return true;
        }
        catch (AmazonS3Exception ex)
        {
            if (ex.StatusCode == HttpStatusCode.NotFound)
                return false;

            throw; // כל שגיאה אחרת תיזרק הלאה
        }
    }
}
