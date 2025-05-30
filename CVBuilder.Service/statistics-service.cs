using CVBuilder.Core.DTOs;
using CVBuilder.Core.Repositories;
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
        private readonly IStatisticsRepository _repository;

        public StatisticsService(IStatisticsRepository repository)
        {
            _repository = repository;
        }

        public async Task<DashboardStatisticsDto> GetDashboardStatisticsAsync()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            return new DashboardStatisticsDto
            {
                TotalUsers = await _repository.GetTotalUsersAsync(),
                TotalCVs = await _repository.GetTotalCVsAsync(),
                TotalTemplates = await _repository.GetTotalTemplatesAsync(),
                ActiveUsers = await _repository.GetActiveUsersAsync(),
                BlockedUsers = await _repository.GetBlockedUsersAsync(),
                CVsCreatedToday = await _repository.GetCVsCreatedTodayAsync(today, tomorrow)
            };
        }

    }
}
