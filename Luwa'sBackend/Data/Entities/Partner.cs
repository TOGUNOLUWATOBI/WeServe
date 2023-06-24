using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace LuwasBackend.Data.Entities
{
    public class Partner : BaseEntity
    {
        public string UserId { get; set; }
        public string Logo { get; set; }
        public string ProfilePicture { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Rating { get; set; }
    }
}
