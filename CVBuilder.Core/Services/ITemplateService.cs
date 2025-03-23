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
        Task<List<string>> GetAllTamplatesAsync();
        Task<string> GetFileAsync(int index);
        //Task<string?> GetFirstFileAsync();
        //Task<List<string>> GetLastFiveFilesAsync();
        //Template? GetTemplateByIdAndUserId(int id, int userId); 
        void AddTemplate(Template template);
        bool DeleteTemplate(int id);
    }
}
