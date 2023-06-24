using Luwa_sBackend.Data.ReturnedResponse;
using LuwasBackend.Data.Models.RequestModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace LuwasBackend.Services.Interfaces
{
    public interface IMessagingService
    {
        Task<ApiResponse> SendMessage(SendMessageRequestModel model, string theUserId);
        Task<ApiResponse> GetMessagesForAChat(string senderId, string receiverId);
        Task<ApiResponse> GetAllChatsForUser(string senderId);
        Task<ApiResponse> RecordChatFromOtherService(SendMessageRequestModel model);
        
    }
}
