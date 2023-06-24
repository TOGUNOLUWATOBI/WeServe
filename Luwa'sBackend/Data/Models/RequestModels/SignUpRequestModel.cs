using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Luwa_sBackend.Data.Models.RequestModels
{
    public class SignUpRequestModel
    {
        [Required]
        public string Firstname { get; set; }
        [Required]
        public string Lastname { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        public string LGA { get; set; }
        public string Address { get; set; }
        public string State { get; set; }

        [Required]
        public string Gender { get; set; }
    }



    public class SignUpPartnerRequestModel
    {
        [Required]
        public string Firstname { get; set; }
        [Required]
        public string Lastname { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        public string LGA { get; set; }
        public string Address { get; set; }
        public string State { get; set; }

        [Required]
        public string Gender { get; set; }


        public IFormFile Logo { get; set; }
        public IFormFile ProfilePicture { get; set; }
        public string Description {get; set; }
        public string Category { get; set; }
    }
}
