using Luwa_sBackend.Data.Models.RequestModels;
using Luwa_sBackend.Data.ReturnedResponse;
using LuwasBackend.Data.Entities;
using Microsoft.AspNetCore.Builder;
using System.Threading.Tasks;

namespace Luwa_sBackend.Services.Interfaces
{
    public interface IMailService
    {
        Task<ApiResponse> SendGenericEmailAsync(MailRequestModel model);
        Task<ApiResponse> SendVerificationEmailAsync(ApplicationUser user, string otpPurpose);
        Task<ApiResponse> SendForgotPasswordEmailAsync(ApplicationUser user, string otpPurpose);
    }
}
