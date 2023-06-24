using System.Xml.Linq;
using System.Xml;
using System;

namespace LuwasBackend.Data.Entities
{
    public class Otp : BaseEntity
    {
        public string UserId { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string OtpCode { get; set; }
        public string Email { get; set; }
        public bool IsUsed { get; set; }
        public string Purpose { get; set; }
    }
}
