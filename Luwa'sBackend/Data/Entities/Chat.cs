using System;

namespace LuwasBackend.Data.Entities
{
    public class Chat : BaseEntity
    {
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }

        public string ReceiverName { get; set; }
        public string Message { get; set; }

    }
}
