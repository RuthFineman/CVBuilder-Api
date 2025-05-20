using System;
using System.Collections.Generic;

namespace CVBuilder.Core.DTOs
{
    public class DashboardStatisticsDto
    {
        public int TotalUsers { get; set; }
        public int TotalCVs { get; set; }
        public int TotalTemplates { get; set; }
        public int ActiveUsers { get; set; }
        public int BlockedUsers { get; set; }
        public int CVsCreatedToday { get; set; }
        public int UsersRegisteredToday { get; set; }
    }

    public class MonthlyDataDto
    {
        public string Month { get; set; }
        public int Count { get; set; }
    }

    public class TemplateUsageDto
    {
        public string TemplateName { get; set; }
        public int UsageCount { get; set; }
    }
}
