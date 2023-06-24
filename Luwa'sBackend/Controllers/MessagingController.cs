using Luwa_sBackend.Controllers;
using Luwa_sBackend.Data.Models.RequestModels;
using Luwa_sBackend.Data.ReturnedResponse;
using Luwa_sBackend.Services.Implementation;
using Luwa_sBackend.Services.Interfaces;
using LuwasBackend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using System;
using LuwasBackend.Data.Models.RequestModels;
using Luwa_sBackend.Data.Constants;
using System.Collections.Generic;
using System.Security.Claims;
using LuwasBackend.Data.Models.ResponseModels;
using LuwasBackend.Data.Enums;

namespace LuwasBackend.Controllers
{
    public class MessagingController : Controller
    {
        private readonly IMessagingService messageService;
        public static IWebHostEnvironment _environment;
        private readonly ILogger<MessagingController> log;

        public MessagingController(IMessagingService messageService, ILogger<MessagingController> log, IWebHostEnvironment environment)
        {
            this.messageService = messageService;
            this.log = log;
            _environment = environment;
        }


        
        [HttpPost]
        [Route("api/v1/SendMessage")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequestModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errMessage = string.Join(" | ", ModelState.Values
                                            .SelectMany(v => v.Errors)
                                            .Select(e => e.ErrorMessage));
                    return BadRequest(ReturnedResponse.ErrorResponse(errMessage, null, StatusCodes.ModelError));
                }

                var theUserId = GetUserId(HttpContext.User.Identity as ClaimsIdentity).userId;
                var resp = await messageService.SendMessage(model, theUserId);
                if (resp.Status == Status.Successful.ToString())
                {
                    return Ok(resp);
                }
                else
                {
                    return BadRequest(resp);
                }

            }
            catch (Exception ex)
            {
                var errMessage = ex.Message == null ? ex.InnerException.ToString() : ex.Message;
                log.LogInformation(string.Concat("Error occured in the sending message", errMessage));
                return BadRequest(ReturnedResponse.ErrorResponse($"an error has occured {ex.Message}", null, StatusCodes.ExceptionError));
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/v1/RecordChat")]
        public async Task<IActionResult> RecordChatFromOtherService([FromBody] SendMessageRequestModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errMessage = string.Join(" | ", ModelState.Values
                                            .SelectMany(v => v.Errors)
                                            .Select(e => e.ErrorMessage));
                    return BadRequest(ReturnedResponse.ErrorResponse(errMessage, null, StatusCodes.ModelError));
                }

                var resp = await messageService.RecordChatFromOtherService(model);
                if (resp.Status == Status.Successful.ToString())
                {
                    return Ok(resp);
                }
                else
                {
                    return BadRequest(resp);
                }

            }
            catch (Exception ex)
            {
                var errMessage = ex.Message == null ? ex.InnerException.ToString() : ex.Message;
                log.LogInformation(string.Concat("Error occured in the SignUp", errMessage));
                return BadRequest(ReturnedResponse.ErrorResponse($"an error has occured: {ex.Message}", null, StatusCodes.ExceptionError));
            }
        }

        [HttpGet]
        [Route("api/v1/GetChats")]
        public async Task<IActionResult> GetChats()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errMessage = string.Join(" | ", ModelState.Values
                                            .SelectMany(v => v.Errors)
                                            .Select(e => e.ErrorMessage));
                    return BadRequest(ReturnedResponse.ErrorResponse(errMessage, null, StatusCodes.ModelError));
                }

                var theUserId = GetUserId(HttpContext.User.Identity as ClaimsIdentity).userId;
                var resp = await messageService.GetAllChatsForUser(theUserId);
                if (resp.Status == Status.Successful.ToString())
                {
                    return Ok(resp);
                }
                else
                {
                    return BadRequest(resp);
                }

            }
            catch (Exception ex)
            {
                var errMessage = ex.Message == null ? ex.InnerException.ToString() : ex.Message;
                log.LogInformation(string.Concat($"Error occured in the getting chats for a user ", errMessage));
                return BadRequest(ReturnedResponse.ErrorResponse($"an error has occured: {ex.Message}", null, StatusCodes.ExceptionError));
            }
        }

        [HttpGet]
        [Route("api/v1/GetMessagesForChat")]
        public async Task<IActionResult> GetMessagesForChat(string receipentId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errMessage = string.Join(" | ", ModelState.Values
                                            .SelectMany(v => v.Errors)
                                            .Select(e => e.ErrorMessage));
                    return BadRequest(ReturnedResponse.ErrorResponse(errMessage, null, StatusCodes.ModelError));
                }

                var theUserId = GetUserId(HttpContext.User.Identity as ClaimsIdentity).userId;
                var resp = await messageService.GetMessagesForAChat(theUserId,receipentId);
                if (resp.Status == Status.Successful.ToString())
                {
                    return Ok(resp);
                }
                else
                {
                    return BadRequest(resp);
                }

            }
            catch (Exception ex)
            {
                var errMessage = ex.Message == null ? ex.InnerException.ToString() : ex.Message;
                log.LogInformation(string.Concat($"Error occured in the getting user ", errMessage));
                return BadRequest(ReturnedResponse.ErrorResponse("an error has occured", null, StatusCodes.ExceptionError));
            }
        }

        private JwtResponseModel GetUserId(ClaimsIdentity identity)
        {

            // Gets list of claims.
            IEnumerable<Claim> claim = identity.Claims;

            // Gets name from claims. Generally it's an email address.
            var userId = claim.Where(x => x.Type == "userId")
            .FirstOrDefault().Value;

            var Role = claim
                .Where(x => x.Type == ClaimTypes.Role)
                .FirstOrDefault().Value;

            return new JwtResponseModel() { userId = userId, role = Role };
        }

    }
}
