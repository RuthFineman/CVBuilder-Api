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
        private readonly IUserRepository _userRepository;

        public TemplateService(ITemplateRepository templateRepository, IUserRepository userRepository)
        {
            _templateRepository = templateRepository;
            _userRepository = userRepository;
        }

        public IEnumerable<Template> GetAllTemplates()
        {
            return _templateRepository.GetAll();
        }
        public Template? GetTemplateByIdAndUserId(int id, int userId)
        {
            return _templateRepository.GetByIdAndUserId(id, userId);
        }

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
