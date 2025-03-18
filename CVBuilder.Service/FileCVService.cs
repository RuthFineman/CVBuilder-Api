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
                Name = fileCV.Name,
                FirstName = fileCV.FirstName,
                LastName = fileCV.LastName,
                Email = fileCV.Email,
                Phone = fileCV.Phone,
                Summary = fileCV.Summary,
                Skills = fileCV.Skills ?? new List<string>(),
                Languages = fileCV.Languages ?? new List<string>()
            };
        }
        public async Task<List<FileCVDto>> GetFilesByUserIdAsync(int userId)
        {
            var files = await _fileCVRepository.GetFilesByUserIdAsync(userId) ?? new List<FileCV>();
            return files.Select(ToFileCVDto).ToList();
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
                Name = fileCVDto.Name ?? "Untitled Resume",
                FirstName = fileCVDto.FirstName,
                LastName = fileCVDto.LastName,
                Email = fileCVDto.Email,
                Phone = fileCVDto.Phone,
                Summary = fileCVDto.Summary,
                Skills = fileCVDto.Skills ?? new List<string>(),
                Languages = fileCVDto.Languages ?? new List<string>()
            };

            await _fileCVRepository.AddAsync(newFile);
            return ToFileCVDto(newFile);
        }
        public async Task<FileCV> UpdateFileCVAsync(int id, int userId, FileCVDto fileCVDto)
        {
            var file = await _fileCVRepository.GetFileByUserIdAsync(id, userId);
            if (file == null) return null;
            file.Id = id;
            file.Name = fileCVDto.Name ?? file.Name;
            file.FirstName = fileCVDto.FirstName ?? file.FirstName;
            file.LastName = fileCVDto.LastName ?? file.LastName;
            file.Email = fileCVDto.Email ?? file.Email;
            file.Phone = fileCVDto.Phone ?? file.Phone;
            file.Summary = fileCVDto.Summary ?? file.Summary;
            file.Skills = fileCVDto.Skills ?? new List<string>();
            file.Languages = fileCVDto.Languages ?? new List<string>();

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
