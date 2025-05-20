using CVBuilder.Core.DTOs;
using CVBuilder.Core.Services;
using CVBuilder.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CVBuilder.Service
{
    public class StatisticsService : IStatisticsService
    {

        //העמוד הזה אמור להתחלק לעוד שכבה  ובגלל שלאעשיתי את זה עדיין אז הוספתי פה הצבעה וזה לא טוב!!!!!!!!!!
        private readonly CVBuilderDbContext _context;

        public StatisticsService(CVBuilderDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardStatisticsDto> GetDashboardStatisticsAsync()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            return new DashboardStatisticsDto
            {
                TotalUsers = await _context.Users.CountAsync(),
                TotalCVs = await _context.FileCVs.CountAsync(),
                TotalTemplates = await _context.Templates.CountAsync(),
                ActiveUsers = await _context.Users.CountAsync(u => !u.IsBlocked),
                BlockedUsers = await _context.Users.CountAsync(u => u.IsBlocked),
                CVsCreatedToday = await _context.FileCVs.CountAsync(cv => cv.UploadedAt >= today && cv.UploadedAt < tomorrow),
                //UsersRegisteredToday = await _context.Users.CountAsync(u => u.Id >= today && u.CreatedAt < tomorrow)
            };
        }

        
        public async Task<List<TemplateUsageDto>> GetTemplateUsageStatisticsAsync()
        {
            var templateUsage = await _context.FileCVs
                .GroupBy(cv => cv.Template)
                .Select(g => new TemplateUsageDto
                {
                    TemplateName = g.Key,
                    UsageCount = g.Count()
                })
                .OrderByDescending(x => x.UsageCount)
                .ToListAsync();

            return templateUsage;
        }

    }
}
