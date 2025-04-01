using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVBuilder.Core.Repositories
{
    public interface IFileRepository
    {
        Task SaveFileRecordAsync(string fileName, string fileUrl);
    }
}
