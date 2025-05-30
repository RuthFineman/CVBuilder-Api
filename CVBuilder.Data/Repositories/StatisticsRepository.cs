using CVBuilder.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVBuilder.Data.Repositories
{
    public class StatisticsRepository : IStatisticsRepository
    {
        private readonly CVBuilderDbContext _context;

        public StatisticsRepository(CVBuilderDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetTotalUsersAsync()
        {
            return await _context.Users.CountAsync();
        }

        public async Task<int> GetTotalCVsAsync()
        {
            return await _context.FileCVs.CountAsync();
        }

        public async Task<int> GetTotalTemplatesAsync()
        {
            return await _context.Templates.CountAsync();
        }

        public async Task<int> GetActiveUsersAsync()
        {
            return await _context.Users.CountAsync(u => !u.IsBlocked);
        }

        public async Task<int> GetBlockedUsersAsync()
        {
            return await _context.Users.CountAsync(u => u.IsBlocked);
        }

        public async Task<int> GetCVsCreatedTodayAsync(DateTime today, DateTime tomorrow)
        {
            return await _context.FileCVs.CountAsync(cv => cv.UploadedAt >= today && cv.UploadedAt < tomorrow);
        }
    }
}
