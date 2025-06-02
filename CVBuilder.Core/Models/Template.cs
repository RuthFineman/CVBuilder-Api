using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVBuilder.Core.Models
{
    public class Template
    {
        [Key]
        public int Id { get; set; }
        public string   Name { get; set; }
        public string TemplateUrl { get; set; }
        public bool InUse{ get; set; }
    }
}

