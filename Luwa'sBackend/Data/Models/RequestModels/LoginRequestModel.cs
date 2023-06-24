using System.ComponentModel.DataAnnotations;

namespace Luwa_sBackend.Data.Models.RequestModels
{
    public class LoginRequestModel
    {
        [Required]
        public string EmailAddress { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
