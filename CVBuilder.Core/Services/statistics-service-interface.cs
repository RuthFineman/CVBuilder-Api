using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CVBuilder.Core.DTOs;

namespace CVBuilder.Core.Services
{
    public interface IStatisticsService
    {
        Task<DashboardStatisticsDto> GetDashboardStatisticsAsync();
        Task<List<TemplateUsageDto>> GetTemplateUsageStatisticsAsync();
    }
}
