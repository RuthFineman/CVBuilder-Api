using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVBuilder.Core.Models
{
    public class FileCV
    {

        public int Id { get; set; }
        [Column(TypeName = "text")]

        public string Name { get; set; }
        public int UserId { get; set; }

        public User User { get; set; }
        [Column(TypeName = "text")]

        public string FilePath { get; set; } = string.Empty;
        [Column(TypeName = "text")]

        public string PathToCss { get; set; }=string.Empty;
        [Column(TypeName = "text")]

        public string FirstName { get; set; } = string.Empty;
        [Column(TypeName = "text")]

        public string LastName { get; set; } = string.Empty;
        [Column(TypeName = "text")]

        public string Email { get; set; } = string.Empty;
        [Column(TypeName = "text")]

        public string Phone { get; set; } = string.Empty;
        [Column(TypeName = "text")]
        public string Summary { get; set; } = string.Empty;

        [Column(TypeName = "text")]

        public List<string> Skills { get; set; } = new List<string>();
        [Column(TypeName = "text")]

        public List<string> Languages { get; set; } = new List<string>();
    }
}
