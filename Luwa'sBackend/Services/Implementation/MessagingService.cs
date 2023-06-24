using AutoMapper;
using Luwa_sBackend.Data;
using Luwa_sBackend.Data.Constants;
using Luwa_sBackend.Data.ReturnedResponse;
using LuwasBackend.Data.Entities;
using LuwasBackend.Data.Models.DTOs;
using LuwasBackend.Data.Models.RequestModels;
using LuwasBackend.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuwasBackend.Services.Implementation
{
    public class MessagingService : IMessagingService
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMapper mapper;


        public MessagingService(ApplicationDbContext context, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        public async Task<ApiResponse> SendMessage(SendMessageRequestModel model, string theUserId)
        {
            try
            {
                var sender = await userManager.FindByIdAsync(theUserId);
                if(sender == null)
                {
                    return ReturnedResponse.ErrorResponse("User not found", null, StatusCodes.NoRecordFound);
                }

                var receipent = await userManager.FindByIdAsync(theUserId);
                if (receipent == null)
                {
                    return ReturnedResponse.ErrorResponse("User not found", null, StatusCodes.NoRecordFound);
                }

                //create new message or chat message
                var Chat = new Chat()
                {
                    CreationDate = DateTime.Now,
                    LastModifiedDate = DateTime.Now,
                    id = Guid.NewGuid(),
                    Message = model.Message,
                    ReceiverId = model.ReceiverId,
                    ReceiverName = receipent.FirstName,
                    SenderId = model.SenderId,
                };

                await context.AddAsync(Chat);
                context.SaveChanges();

                //todo: send push notification to receipent

                //todo: record the message on the other microservice

                return ReturnedResponse.SuccessResponse("Message Sent", model, StatusCodes.Successful);
            }
            catch (Exception ex)
            {
                return ReturnedResponse.ErrorResponse($"An error occured: {ex.Message}", null, StatusCodes.ExceptionError);
            }
        }


        public async Task<ApiResponse> GetMessagesForAChat (string senderId, string receiverId)
        {
            try
            {
                var recordedMessages = await context.Chats.Where(x => x.SenderId.ToString() == senderId && x.ReceiverId.ToString() == receiverId).OrderByDescending(x => x.CreationDate).ToListAsync();

                var messages = new List<MessageDTO>();
                foreach (var message in recordedMessages)
                {
                    var messageDTO = mapper.Map<MessageDTO>(message);
                    messages.Add(messageDTO);
                }

                return ReturnedResponse.SuccessResponse("Messages between 2 users", messages, StatusCodes.Successful);
            }
            catch (Exception ex)
            {
                return ReturnedResponse.ErrorResponse($"An error occured: {ex.Message}", null, StatusCodes.ExceptionError);
            }
        }


        public async Task<ApiResponse> GetAllChatsForUser (string senderId)
        {
            try
            {
                var chats = await context.Chats.Where(x => x.SenderId.ToString() == senderId).GroupBy(x=> x.ReceiverId).Select(x=> new {x.First().ReceiverId,x.First().CreationDate,x.First().Message, x.First().ReceiverName}).ToListAsync();

                return ReturnedResponse.SuccessResponse("All chats", chats, StatusCodes.Successful);
            }
            catch (Exception ex)
            {
                return ReturnedResponse.ErrorResponse($"An error occured: {ex.Message}", null, StatusCodes.ExceptionError);
            }
        }


        public async Task<ApiResponse> RecordChatFromOtherService(SendMessageRequestModel model)
        {
            var receiver = await userManager.FindByIdAsync(model.ReceiverId.ToString());
            if (receiver == null)
            {
                return ReturnedResponse.ErrorResponse("user not found", null, StatusCodes.NoRecordFound);
            }
            try
            {
                //create new message or chat message
                var Chat = new Chat()
                {
                    CreationDate = DateTime.Now,
                    LastModifiedDate = DateTime.Now,
                    id = Guid.NewGuid(),
                    Message = model.Message,
                    ReceiverId = model.ReceiverId,
                    ReceiverName = receiver.FirstName,
                    SenderId = model.SenderId,
                };

                await context.AddAsync(Chat);
                context.SaveChanges();


                return ReturnedResponse.SuccessResponse("Message Sent", model, StatusCodes.Successful);
            }
            catch (Exception ex)
            {
                return ReturnedResponse.ErrorResponse($"An error occured: {ex.Message}", null, StatusCodes.ExceptionError);
            }
        }
    }
    
}
