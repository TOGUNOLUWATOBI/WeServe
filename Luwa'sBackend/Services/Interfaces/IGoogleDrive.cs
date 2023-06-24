using Luwa_sBackend.Data.ReturnedResponse;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace LuwasBackend.Services.Interfaces
{
    public interface IGoogleDrive
    {
        Task<ApiResponse> UploadFileWithMetaData(IFormFile file, string theUserId, string ImageType);
    }
}
