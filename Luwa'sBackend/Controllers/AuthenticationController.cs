using Luwa_sBackend.Data.Models.RequestModels;
using Luwa_sBackend.Data.ReturnedResponse;
using Luwa_sBackend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using Luwa_sBackend.Data.Constants;
using Org.BouncyCastle.Asn1.Ocsp;
using LuwasBackend.Data.Models.ResponseModels;
using LuwasBackend.Services.Interfaces;
using LuwasBackend.Data.Enums;

namespace Luwa_sBackend.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly IAuthentication authService;
        public static IWebHostEnvironment _environment;
        private readonly ILogger<AuthenticationController> log;

        public AuthenticationController(IAuthentication authService,  ILogger<AuthenticationController> log, IWebHostEnvironment environment)
        {
            this.authService = authService;
            this.log = log;
            _environment = environment;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/v1/SignUp")]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequestModel model)
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

                var resp = await authService.CreateUser(model);
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
                return BadRequest(ReturnedResponse.ErrorResponse("an error has occured", null, StatusCodes.ExceptionError));
            }
        }


        [AllowAnonymous]
        [HttpPost]
        [Route("api/v1/SignUpPartner")]
        public async Task<IActionResult> SignUpPartner([FromBody] SignUpPartnerRequestModel model)
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

                var resp = await authService.CreatePartner(model);
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
                return BadRequest(ReturnedResponse.ErrorResponse("an error has occured", null, StatusCodes.ExceptionError));
            }
        }


        [AllowAnonymous]
        [HttpPost]
        [Route("api/v1/Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestModel model)
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

                var resp = await authService.Login(model);
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
                log.LogInformation(string.Concat("Error occured in the Login", errMessage));
                return BadRequest(ReturnedResponse.ErrorResponse("an error has occured", null, StatusCodes.ExceptionError));
            }
        }


        [HttpPost]
        [Route("api/v1/DeactivateUser")]
        public async Task<IActionResult> DeactivateUser(string email)
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

                var resp = await authService.DeActivateUser(email);
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
                log.LogInformation(string.Concat($"Error occured in the Deactivation of user with email: {email}", errMessage));
                return BadRequest(ReturnedResponse.ErrorResponse("an error has occured", null, StatusCodes.ExceptionError));
            }
        }

        //[Authorize]
        [HttpGet]
        [Route("api/v1/Users")]


        public async Task<IActionResult> GetAllUser()
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

                var resp = await authService.GetUsers();
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
                log.LogInformation(string.Concat($"Error occured in retrieving all users", errMessage));
                return BadRequest(ReturnedResponse.ErrorResponse("an error has occured", null, StatusCodes.ExceptionError));
            }
        }

        //[Authorize]
        [HttpGet]
        [Route("api/v1/GetUser")]
        public async Task<IActionResult> GetUser(string email)
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

                var resp = await authService.GetUser(email);
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


        [HttpGet]
        [Route("api/v1/GetUserById")]
        public async Task<IActionResult> GetUserById(string id)
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

                var resp = await authService.GetUserById(id);
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


        [AllowAnonymous]
        [HttpPost]
        [Route("api/v1/VerifyOtp")]
        public async Task<IActionResult> VerifyOtp(string otpCode)
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

                var resp = await authService.VerifyOtp(otpCode);
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
                log.LogInformation(string.Concat($"Error occured in verifying otp ", errMessage));
                return BadRequest(ReturnedResponse.ErrorResponse("an error occured in verifying otp", null, StatusCodes.ExceptionError));
            }
        }



        [AllowAnonymous]
        [HttpPost]
        [Route("api/v1/ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ChangePasswordRequestModel model)
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

                var resp = await authService.ResetPassword(model);
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
                log.LogInformation(string.Concat($"Error occured in resetting password", errMessage));
                return BadRequest(ReturnedResponse.ErrorResponse("an error occured in resseting password", null, StatusCodes.ExceptionError));
            }
        }



        [AllowAnonymous]
        [HttpPost]
        [Route("api/v1/ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(string email)
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

                var resp = await authService.ForgotPasswordRequest(email);
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
                log.LogInformation(string.Concat($"Error occured in resetting password", errMessage));
                return BadRequest(ReturnedResponse.ErrorResponse("an error occured in resseting password", null, StatusCodes.ExceptionError));
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/v1/ResendOtp")]
        public async Task<IActionResult> ResendOtp(string email)
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

                var resp = await authService.ResendOTP(email);
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
                log.LogInformation(string.Concat($"Error occured in resending otp", errMessage));
                return BadRequest(ReturnedResponse.ErrorResponse($"an error occured in resending otp: {ex.Message}", null, StatusCodes.ExceptionError));
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

            return new JwtResponseModel(){ userId = userId, role = Role };
        }
    }
}
