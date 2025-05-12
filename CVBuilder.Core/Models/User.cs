using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVBuilder.Core.Models
{
    public class User
    {
        public int Id { get; set; }
        public bool IsAdmin { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public List<FileCV> CVFiles { get; set; }
    }
}
