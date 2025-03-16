using CVBuilder.Core.Models;
using CVBuilder.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVBuilder.Data.Repositories
{
    public class TemplateRepository: ITemplateRepository
    {
        private readonly CVBuilderDbContext _context;

        public TemplateRepository(CVBuilderDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Template> GetAll()
        {
            return _context.Templates.ToList();  
        }
        //שיניתי כאן משהו בפונקציה למטה

        public Template? GetByIdAndUserId(int id, int userId)
        {
            return _context.Templates.FirstOrDefault(t => t.Id == id );
        }

        public void Add(Template template)
        {
            _context.Templates.Add(template);
            _context.SaveChanges();
        }

        public bool Delete(int id)
        {
            var template = _context.Templates.Find(id);
            if (template == null)
                return false;

            _context.Templates.Remove(template);
            _context.SaveChanges();
            return true;
        }
    }
}
