using System;

namespace LuwasBackend.Data.Models.DTOs
{
    public class MessageDTO
    {
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public string Message { get; set; }
        public  DateTime CreationDate { get; set; }
        public string ReceiverName { get; set; }
    }
}
