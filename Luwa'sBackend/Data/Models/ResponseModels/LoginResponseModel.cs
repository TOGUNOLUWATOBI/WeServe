using System;

namespace Luwa_sBackend.Data.Models.ResponseModels
{
    public class LoginResponseModel
    {
        public string Email { get; set; }
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public DateTime ExpiresIn { get; set; }

    }
}
