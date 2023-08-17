using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Utils.Azure
{
    public interface IAzureStorage
    {
        Task<string> UploadAsync(IFormFile file);
        Task<bool> DeleteAsync(string blobFilename);
    }
}
