using Luwa_sBackend.Data.ReturnedResponse;
using LuwasBackend.Data.Enums;
using LuwasBackend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System;
using System.Threading.Tasks;
using Luwa_sBackend.Data.Constants;

namespace LuwasBackend.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IAuthentication authService;
        public static IWebHostEnvironment _environment;
        private readonly ILogger<DashboardController> log;

        public DashboardController(IAuthentication authService, ILogger<DashboardController> log, IWebHostEnvironment environment)
        {
            this.authService = authService;
            this.log = log;
            _environment = environment;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/v1/getCategories")]
        public async Task<IActionResult> GetCategories()
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

                var resp = Enum.GetNames(typeof(Categories));
                return Ok(ReturnedResponse.SuccessResponse("Categories",resp,StatusCodes.Successful));
                
            }
            catch (Exception ex)
            {
                var errMessage = ex.Message == null ? ex.InnerException.ToString() : ex.Message;
                log.LogInformation(string.Concat($"Error occured in retrieving all users", errMessage));
                return BadRequest(ReturnedResponse.ErrorResponse("an error has occured", null, StatusCodes.ExceptionError));
            }
        }
    }
}
