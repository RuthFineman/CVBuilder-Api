using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
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
    public class TemplateService: ITemplateService
    {
        private readonly ITemplateRepository _templateRepository;
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName = "cvfilebuilder";

        public TemplateService(ITemplateRepository ITemplateRepository)
        {
            _templateRepository = ITemplateRepository;
        }
        public async Task<List<string>> GetAllFilesAsync()
        {
            return await _templateRepository.GetAllFilesAsync();
        }
        public async Task<string?> GetFirstFileAsync()
        {
            return await _templateRepository.GetFirstFileAsync();
        }
        //public Template? GetTemplateByIdAndUserId(int id, int userId)
        //{
        //    return _templateRepository.GetByIdAndUserId(id, userId);
        //}

        public void AddTemplate(Template template)
        {
            _templateRepository.Add(template);
        }

        public bool DeleteTemplate(int id)
        {
            return _templateRepository.Delete(id);
        }
    }
}
