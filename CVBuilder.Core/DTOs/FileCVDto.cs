using CVBuilder.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVBuilder.Core.DTOs
{
    public class FileCVDto
    {
        [Column(TypeName = "text")]
        public string Name { get; set; }

        [Column(TypeName = "text")]
        public string? FirstName { get; set; }

        [Column(TypeName = "text")]
        public string? LastName { get; set; }

        [Column(TypeName = "text")]
        public string? Email { get; set; }

        [Column(TypeName = "text")]
        public string? Phone { get; set; }
     
        [Column(TypeName = "text")]
        public string? Summary { get; set; }

        [Column(TypeName = "text")]
        public List<string>? Skills { get; set; } = new List<string>();

        [Column(TypeName = "text")]
        public List<string>? Languages { get; set; } = new List<string>();
    }
}
