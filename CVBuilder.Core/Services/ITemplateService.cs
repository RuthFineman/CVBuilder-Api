using CVBuilder.Core.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVBuilder.Core.Services
{
    public interface ITemplateService
    {
        //הוספת תבנית
        Task<string> AddTemplateAsync(IFormFile file, string fileName);
        Task<bool> DeleteTemplateAsync(string fileName);
        Task<List<string>> GetAllTamplatesAsync();
        Task<string> GetFileAsync(int index);
        Task<Template?> UpdateTemplateStatusAsync(int id, bool newStatus);

        Task<int?> GetTemplateIdByUrlAsync(string url);

    }
}
