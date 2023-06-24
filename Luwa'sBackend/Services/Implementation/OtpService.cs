using Luwa_sBackend.Core;
using Luwa_sBackend.Data;
using Luwa_sBackend.Services.Interfaces;
using LuwasBackend.Data.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Luwa_sBackend.Services.Implementation
{
    public class OtpService : IOtpService
    {
        private readonly ApplicationDbContext context;

        public OtpService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<string> GenerateOtpCodeAsync()
        {
            var otpcode = Utility.GenerateOtpCode();

            var otp = await context.Otps.FirstOrDefaultAsync(x => x.OtpCode == otpcode);
            while (otp != null)
            {

                otpcode = Utility.GenerateOtpCode();
                otp = await context.Otps.FirstOrDefaultAsync(x => x.OtpCode.Equals(otpcode));
            }

            return otpcode;
        }

        public async Task<bool> ReSendOtpCode(string email)
        {
            var otp = await context.Otps.FirstOrDefaultAsync(x => x.Email == email);
            if (otp != null)
            {

                otp.IsUsed = false;
                otp.OtpCode = await GenerateOtpCodeAsync();
                otp.ExpiryDate = DateTime.Now.AddMinutes(10);
                otp.LastModifiedDate = DateTime.Now;

                return true;
            }
            return false;
        }

        public async Task<bool> VerifyOtpCodeValidityAsync(string otpCode)
        {
            var otp = await context.Otps.FirstOrDefaultAsync(x => x.OtpCode == otpCode);
            if (otp == null)
                return false;
            if (otp.IsUsed)
                return false;
            if (otp.ExpiryDate < DateTime.Now)
                return false;
            if (otp.Purpose == OtpPurpose.UserVerification.ToString())
            {
                var user = await context.Users.FirstOrDefaultAsync(x => x.Id == otp.UserId);
                user.Status = UserStatus.Active.ToString();
                context.Entry(user).State = EntityState.Modified;
            }
            otp.IsUsed = true;
            otp.LastModifiedDate = DateTime.Now;
            context.Entry(otp).State = EntityState.Modified;

            await context.SaveChangesAsync();

            return true;
        }
    }
}
