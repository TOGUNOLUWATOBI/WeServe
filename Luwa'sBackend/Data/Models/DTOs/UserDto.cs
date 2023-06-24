using LuwasBackend.Data.Entities;
using System.Collections.Generic;
using System;

namespace Luwa_sBackend.Data.Models.DTOs
{
    public class UserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public string Status { get; set; }
        public string Email { get; set; }
    }


    public class Dashboardpartners
    {
        public string Name { get; set; }
        public List<Partner> Parnters { get; set; }

        
    }

}
