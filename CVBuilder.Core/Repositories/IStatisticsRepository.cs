using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVBuilder.Core.Repositories
{
    public interface IStatisticsRepository
    {
        Task<int> GetTotalUsersAsync();
        Task<int> GetTotalCVsAsync();
        Task<int> GetTotalTemplatesAsync();
        Task<int> GetActiveUsersAsync();
        Task<int> GetBlockedUsersAsync();
        Task<int> GetCVsCreatedTodayAsync(DateTime today, DateTime tomorrow);
    }
}
