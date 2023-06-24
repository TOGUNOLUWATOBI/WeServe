using System.ComponentModel.DataAnnotations;

namespace Luwa_sBackend.Data.Models.RequestModels
{
    public class ChangePasswordRequestModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
