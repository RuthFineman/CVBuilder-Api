using CVBuilder.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVBuilder.Core.Services
{
    public interface IUserService
    {
        Task<bool> RegisterAsync(string fullName, string email, string password, string role);
        Task<User> LoginAsync(string email, string password);
    }
}
