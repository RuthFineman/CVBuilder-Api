using CVBuilder.Core.DTOs;
using CVBuilder.Core.Models;
using CVBuilder.Core.Repositories;
using CVBuilder.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVBuilder.Service
{
    public class FileCVService : IFileCVService
    {
        private readonly IFileCVRepository _fileCVRepository;
        public FileCVService(IFileCVRepository fileCVRepository)
        {
            _fileCVRepository = fileCVRepository;
        }
        private FileCVDto ToFileCVDto(FileCV fileCV)
        {
            return new FileCVDto
            {
                Id = fileCV.Id,
                FirstName = fileCV.FirstName,
                LastName = fileCV.LastName,
                Email = fileCV.Email,
                Phone = fileCV.Phone,
                Summary = fileCV.Summary,
                WorkExperiences = fileCV.WorkExperiences?.Select(we => new CVBuilder.Core.DTOs.WorkExperience
                {
                    Company = we.Company,
                    Position = we.Position,
                    StartDate = we.StartDate,
                    EndDate = we.EndDate,
                    Description = we.Description
                }).ToList() ?? new List<CVBuilder.Core.DTOs.WorkExperience>(),  
                Educations = fileCV.Educations?.Select(e => new CVBuilder.Core.DTOs.Education
                {
                    Institution = e.Institution,
                    Degree = e.Degree
                }).ToList() ?? new List<CVBuilder.Core.DTOs.Education>(),  
                Skills = fileCV.Skills,
            };
        }
        public async Task<List<string>> GetUserFilesAsync(string userId)
        {
            return await _fileCVRepository.FetchFilesByUserIdAsync(userId);
        }
        public async Task<bool> DeleteFileByUserIdAsync(int fileId, int userId)
        {
            var file = await _fileCVRepository.GetFileByUserIdAsync(fileId, userId);
            if (file == null) return false;

            await _fileCVRepository.DeleteFileCVAsync(file.Id);
            return true;
        }
        public async Task<FileCVDto> CreateFileCVAsync(FileCVDto fileCVDto, int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("UserId Missing");
            }

            var userFiles = await _fileCVRepository.GetByUserIdAsync(userId);
            if (userFiles.Count >= 5)
            {
                throw new InvalidOperationException("User cannot create more than 5 resumes.");
            }

            var newFile = new FileCV
            {
                UserId = userId,
                FirstName = fileCVDto.FirstName,
                LastName = fileCVDto.LastName,
                Email = fileCVDto.Email,
                Phone = fileCVDto.Phone,  
                Summary = fileCVDto.Summary,
                //WorkExperiences = fileCVDto.WorkExperiences,
                //Educations = fileCVDto.Educations,
                Skills = fileCVDto.Skills ?? new List<string>(),
                // שדות אחרים שאולי הוספתם
            };

            await _fileCVRepository.AddAsync(newFile);
            return ToFileCVDto(newFile);
        }

        public async Task<FileCV> UpdateFileCVAsync(int id, int userId, FileCVDto fileCVDto)
        {
            var file = await _fileCVRepository.GetFileByUserIdAsync(id, userId);
            if (file == null) return null;

            file.Id = id;
            file.FirstName = fileCVDto.FirstName ?? file.FirstName;
            file.LastName = fileCVDto.LastName ?? file.LastName;
            file.Email = fileCVDto.Email ?? file.Email;
            file.Phone = fileCVDto.Phone ?? file.Phone;
            file.Summary = fileCVDto.Summary ?? file.Summary;
            file.Skills = fileCVDto.Skills ?? new List<string>();

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

            await _fileCVRepository.UpdateAsync(file);
            return file;
        }
        public async Task<FileCVDto> GetFileByUserIdAsync(int fileId, int userId)
        {
            var file = await _fileCVRepository.GetFileByUserIdAsync(fileId, userId);
            return file == null ? null : ToFileCVDto(file);
        }
    }

}
