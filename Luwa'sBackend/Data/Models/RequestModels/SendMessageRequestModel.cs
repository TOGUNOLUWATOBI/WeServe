using System;

namespace LuwasBackend.Data.Models.RequestModels
{
    public class SendMessageRequestModel
    {
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public string Message { get; set; }
    }
}
