using Microsoft.AspNetCore.Identity;
using System;

namespace LuwasBackend.Data.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public string Status { get; set; }
        
        public bool IsEmailConfirmed { get; set; }

        

        
        public DateTime? CreationDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }

    }
}
