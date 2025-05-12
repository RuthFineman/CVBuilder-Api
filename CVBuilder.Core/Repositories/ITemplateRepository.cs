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
        //העלאת תבנית
        Task AddTemplateAsync(Template template);
        Task DeleteTemplateAsync(string fileName);
        Task<List<string>> GetAllTemplateUrlsAsync();
        Task<string> GetFileNameByIndexAsync(int index);
        //Task<List<string>> GetAllTamplatesAsync();
        //Task<string> GetFileByIndexAsync(int index);
        //Task<string?> GetFirstFileAsync();
        //Template? GetByIdAndUserId(int id, int userId);
        void Add(Template template);
        bool Delete(int id);
    }
}
