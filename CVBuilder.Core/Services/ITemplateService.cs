using CVBuilder.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVBuilder.Core.Services
{
    public interface ITemplateService
    {
        IEnumerable<Template> GetAllTemplates();
        Template? GetTemplateByIdAndUserId(int id, int userId);
        void AddTemplate(Template template);
        bool DeleteTemplate(int id);
    }
}
