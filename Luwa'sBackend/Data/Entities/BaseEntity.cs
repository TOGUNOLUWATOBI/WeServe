using System;

namespace LuwasBackend.Data.Entities
{
    public class BaseEntity
    {
        public Guid id { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
