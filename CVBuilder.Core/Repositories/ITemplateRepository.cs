using CVBuilder.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVBuilder.Core.Repositories
{
    public interface ITemplateRepository
    {
        Task AddTemplateAsync(Template template);
        Task DeleteTemplateAsync(string fileName);
        Task<List<Template>> GetAllTemplatesFromDbAsync();
        Task<string> GetFileNameByIndexAsync(int index);
        Task<Template?> GetByIdAsync(int id);
        Task UpdateAsync(Template template);
    }
}
