﻿using CVBuilder.Core.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CVBuilder.Core.Models
{
    public class FileCV
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public DateTime UploadedAt { get; set; }
        public User User { get; set; }
        public string FilePath { get; set; } = string.Empty;
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }
        [JsonPropertyName("lastName")]
        public string LastName { get; set; }
        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("Phone")]
        public string Phone { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("summary")]
        public string Summary { get; set; }

        [JsonPropertyName("workExperiences")]
        public List<WorkExperience> WorkExperiences { get; set; } = new List<WorkExperience>();
        [JsonPropertyName("languages")]
        public List<Language> Languages { get; set; } = new List<Language>();

        [JsonPropertyName("educations")]
        public List<Education> Educations { get; set; } = new List<Education>();

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
        [Key] // הוספת מפתח ראשי
        public int Id { get; set; }

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
    public class Language
    {
        [Key]
        [JsonPropertyName("languageName")]
        public string LanguageName { get; set; }

        [JsonPropertyName("proficiency")]
        public string Proficiency { get; set; }
    }

    public class Education
    {
        [Key]
        public int Id { get; set; }  
        [JsonPropertyName("institution")]
        public string Institution { get; set; }

        [JsonPropertyName("degree")]
        public string Degree { get; set; }

    }
}

