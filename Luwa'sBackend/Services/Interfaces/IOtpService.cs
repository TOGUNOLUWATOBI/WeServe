
using System.Threading.Tasks;

namespace Luwa_sBackend.Services.Interfaces
{
    public interface IOtpService
    {
        Task<string> GenerateOtpCodeAsync();

        Task<bool> VerifyOtpCodeValidityAsync(string code);

        Task<bool> ReSendOtpCode(string email);

    }
}
