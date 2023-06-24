using Luwa_sBackend.Data.Models.RequestModels;
using Luwa_sBackend.Data.ReturnedResponse;
using System.Threading.Tasks;

namespace LuwasBackend.Services.Interfaces
{
    public interface IAuthentication
    {
        Task<ApiResponse> CreateUser(SignUpRequestModel model);
        Task<ApiResponse> CreatePartner(SignUpPartnerRequestModel model);
        Task<ApiResponse> UpdateUser();
        Task<ApiResponse> DeActivateUser(string email);
        Task<ApiResponse> ActivateUser(string token);
        Task<ApiResponse> GetUser(string email);
        Task<ApiResponse> GetUserById(string id);
        Task<ApiResponse> GetUsers();
        Task<ApiResponse> Login(LoginRequestModel model);

        Task<ApiResponse> ForgotPasswordRequest(string email);
        Task<ApiResponse> ResetPassword(ChangePasswordRequestModel model);
        Task<ApiResponse> VerifyOtp(string otpCode);
        Task<ApiResponse> ResendOTP(string email);

    }
}
