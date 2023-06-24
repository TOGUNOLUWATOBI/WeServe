using static System.Net.WebRequestMethods;
using System.Collections.Generic;
using System.Security.Principal;
using System.Transactions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LuwasBackend.Data.Entities;

namespace Luwa_sBackend.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Otp> Otps { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Partner> Partners { get; set; }
        
        public ApplicationDbContext()
        {

        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

    }
}
