using Amazon.S3.Model;
using Amazon.S3;
using CVBuilder.Core.Services;
using Microsoft.AspNetCore.Http;
using CVBuilder.Core.DTOs;
using CVBuilder.Core.Models;
using CVBuilder.Core.Repositories;
using System.Net;
using Microsoft.EntityFrameworkCore;

public class FileCVService : IFileCVService
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName = "cvfilebuilder";
    private readonly IFileCVRepository _fileCVRepository;  

    public FileCVService(IAmazonS3 s3Client, IFileCVRepository fileRepository)
    {
        _s3Client = s3Client;
        _fileCVRepository = fileRepository;
    }
    private FileCV ConvertToFileCV(FileCVDto fileDto, string userId, IFormFile file)
    {
        return new FileCV
        {
            UserId = int.Parse(userId),
            FileName = file.FileName,
            FileUrl = $"https://{_bucketName}.s3.amazonaws.com/{userId}/{file.FileName}",
            UploadedAt = DateTime.UtcNow,
            Template=fileDto.Template,
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
                Level = l.Level
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
        var dbFiles = await _fileCVRepository.GetFilesByUserIdAsync(userId);
        var s3Files = await FetchFilesFromS3Async(userId); 

        var s3Dict = s3Files.ToDictionary(
            key => Path.GetFileName(key), 
            key => key 
        );
        var merged = dbFiles.Select(f => new {
            id = f.Id.ToString(),
            fileName = f.FileName,
            path = s3Dict.ContainsKey(f.FileName) ? s3Dict[f.FileName] : null
        }).ToList();

        return merged.Cast<object>().ToList();
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
            throw new ArgumentException("UserId is required");

        if (file == null || file.Length == 0)
            throw new InvalidOperationException("No file provided or file is empty");

        if (fileDto == null)
            throw new ArgumentException("FileCVDto cannot be null");

        var fileRecord = ConvertToFileCV(fileDto, userId, file);
        if (fileRecord == null)
            throw new InvalidOperationException("Failed to convert FileCVDto to FileCV.");

        if (string.IsNullOrEmpty(fileRecord.Email))
            throw new ArgumentException("Email is required.");

        try
        {
            await _fileCVRepository.SaveFileRecordAsync(fileRecord);

            using var stream = file.OpenReadStream();
            if (stream.Length == 0)
                throw new InvalidOperationException("The file is empty.");

            var uploadRequest = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = $"{userId}/{file.FileName}",
                InputStream = stream,
                ContentType = file.ContentType
            };

            await _s3Client.PutObjectAsync(uploadRequest);
        }
        catch (Exception ex)
        {
            if (fileRecord?.Id != 0)
            {
                await _fileCVRepository.DeleteFileCVAsync(fileRecord.Id);
            }

            throw new InvalidOperationException("An error occurred while saving the file or uploading to AWS.", ex);
        }
    }

    public async Task<bool> DeleteFileByUserIdAsync(int fileId, string userId)
    {
        var file = await _fileCVRepository.GetFileByUserIdAsync(fileId, userId);
        if (file == null) return false; 

        try
        {
            await _fileCVRepository.DeleteFileCVAsync(file.Id);
        }
        catch (Exception ex)
        {
            return false;
        }

        if (!string.IsNullOrEmpty(file.FileName))
        {
            await _s3Client.DeleteObjectAsync(_bucketName, $"{userId}/{file.FileName}");
        }

        return true; 
    }
    public async Task<FileCV> UpdateFileCVAsync(IFormFile newFile, int id, string userId, FileCVDto fileCVDto)
    {
        

        var oldFile = await _fileCVRepository.GetFileByUserIdAsync(id, userId);
        if (oldFile == null) return null;

        var oldKey = $"{userId}/{oldFile.FileName}";
        try
        {
            if (!string.IsNullOrEmpty(fileCVDto.FileName))
            {
                await _s3Client.DeleteObjectAsync(_bucketName, oldKey);
            }
        }
        catch (AmazonS3Exception e)
        {
            if (e.ErrorCode != "NoSuchKey")
            {
                throw; 
            }
        }
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
        var file = await _fileCVRepository.GetFileByUserIdAsync(id, userId);
        if (file == null) return null;
        file.Id = id;
        file.FirstName = fileCVDto.FirstName ?? file.FirstName;
        file.LastName = fileCVDto.LastName ?? file.LastName;
        file.Role = fileCVDto.Role ?? file.Role;
        file.Template = fileCVDto.Template ?? file.Template;
        file.FileName = newFile.FileName;
        file.Email = fileCVDto.Email ?? file.Email;
        file.Phone = fileCVDto.Phone ?? file.Phone;
        file.Summary = fileCVDto.Summary ?? file.Summary;
        if (fileCVDto.Skills != null)
            file.Skills = fileCVDto.Skills;

        file.Languages = fileCVDto.Languages?.Select(l => new CVBuilder.Core.Models.Language
        {
            LanguageName = l.LanguageName,
            Level = l.Level
        }).ToList() ?? new List<CVBuilder.Core.Models.Language>();

        file.WorkExperiences = fileCVDto.WorkExperiences?.Select(we => new CVBuilder.Core.Models.WorkExperience
        {
            Company = we.Company,
            Position = we.Position,
            StartDate = we.StartDate,
            EndDate = we.EndDate,
            Description = we.Description
        }).ToList() ?? new List<CVBuilder.Core.Models.WorkExperience>(); 
        file.Educations = fileCVDto.Educations?.Select(e => new CVBuilder.Core.Models.Education
        {
            Institution = e.Institution,
            Degree = e.Degree
        }).ToList() ?? new List<CVBuilder.Core.Models.Education>();  
        await _fileCVRepository.UpdateAsync(file);
        return file;
    }
    public async Task<FileCV> GetFileCVByIdAsync(int id, string userId)
    {
        return await _fileCVRepository.GetFileByUserIdAsync(id, userId);
    }
    public async Task<List<FileCVStatsDto>> GetUploadStats()
    {
        return await _fileCVRepository.GetUploadStatsAsync();
    }
}
