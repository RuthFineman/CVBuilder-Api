using CVBuilder.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CVBuilder.Core.DTOs
{
    public class FileCVDto
    {
        public int Id { get; set; }

        [JsonPropertyName("firstName")]
        public string FileName { get; set; }
        public string FirstName { get; set; }
        [JsonPropertyName("lastName")]
        public string LastName { get; set; }

        [JsonPropertyName("Phone")]
        public string Phone { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("summary")]
        public string Summary { get; set; }

        [JsonPropertyName("workExperiences")]
        public List<WorkExperience> WorkExperiences { get; set; }

        [JsonPropertyName("educations")]
        public List<Education> Educations { get; set; }

        [JsonPropertyName("skills")]
        public List<string> Skills { get; set; } = new List<string>
        {
            "כישורי ארגון",
            "פתרון בעיות",
            "עבודה בצוות",
            "יצירתיות",
            "אחריות",
            "תפקוד במצבי לחץ",
            "מוסר עבודה גבוה",
            "ניהול זמן יעיל",
            "חשיבה אנליטית",
            "יחסי אנוש מעולים"
        };
    }

    public class WorkExperience
    {
        [JsonPropertyName("company")]
        public string Company { get; set; }

        [JsonPropertyName("position")]
        public string Position { get; set; }

        [JsonPropertyName("startDate")]
        public string StartDate { get; set; }

        [JsonPropertyName("endDate")]
        public string EndDate { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
    public class Education
    {

        [JsonPropertyName("institution")]
        public string Institution { get; set; }

        [JsonPropertyName("degree")]
        public string Degree { get; set; }
    }
}
