using Luwa_sBackend.Data.ReturnedResponse;
using System.Threading.Tasks;

namespace LuwasBackend.Services.Interfaces
{
    public interface IDashBoardService
    {
        Task<ApiResponse> GetPartnersByCategory(string category, int pageNumber = 1, int pageSize = 10, string? filter = null);
        Task<ApiResponse> Search(string keyword);
        Task<ApiResponse> GetDashboardHome();
    }
}
