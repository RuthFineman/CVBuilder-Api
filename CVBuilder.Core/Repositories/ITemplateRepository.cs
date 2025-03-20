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
        Task<List<string>> GetAllFilesAsync();
        Task<string?> GetFirstFileAsync();
        Template? GetByIdAndUserId(int id, int userId);
        void Add(Template template);
        bool Delete(int id);
    }
}
