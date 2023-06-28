using AutoMapper;
using Luwa_sBackend.Data.Models.DTOs;
using Luwa_sBackend.Data.Models.RequestModels;
using Luwa_sBackend.Data.Models.ResponseModels;
using Luwa_sBackend.Data.ReturnedResponse;
using Luwa_sBackend.Data;
using Luwa_sBackend.Settings;
using LuwasBackend.Data.Constants;
using LuwasBackend.Data.Entities;
using LuwasBackend.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System;
using Luwa_sBackend.Data.Constants;
using Microsoft.EntityFrameworkCore;
using Luwa_sBackend.Services.Interfaces;
using LuwasBackend.Data.Enums;
using Microsoft.AspNetCore.Mvc;

namespace LuwasBackend.Services.Implementation
{
    public class Authentication : IAuthentication
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMapper map;
        private readonly IGoogleDrive drive;
        private readonly IMailService mailService;
        private readonly IOtpService otpService;
        private readonly RoleManager<IdentityRole> roleManager;

        private readonly AppSettings _appSettings;


        public Authentication(UserManager<ApplicationUser> userManager, ApplicationDbContext context, SignInManager<ApplicationUser> signInManager, IMapper map, IMailService mailService, IOptions<AppSettings> appSettings, RoleManager<IdentityRole> roleManager, IOtpService otpService, IGoogleDrive drive)
        {
            _userManager = userManager;
            this.context = context;
            _signInManager = signInManager;
            this.map = map;
            this.mailService = mailService;
            _appSettings = appSettings.Value;
            this.roleManager = roleManager;
            this.otpService = otpService;
            this.drive = drive;
        }


        public async Task<bool> CreateRoles()
        {
            if (!await roleManager.RoleExistsAsync(UserRoles.User))
            {
                await roleManager.CreateAsync(new IdentityRole(UserRoles.User));
                return true;
            }

            if (!await roleManager.RoleExistsAsync(UserRoles.Partners))
            {
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Partners));
                return true;
            }

            if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
                return true;
            }

            return true;

        }
        public async Task<ApiResponse> ActivateUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return ReturnedResponse.ErrorResponse("User not found", null, StatusCodes.NoRecordFound);

            }
            if (user.Status == UserStatus.Blacklisted.ToString())
            {
                return ReturnedResponse.ErrorResponse("Can't activate user as user is blacklisted", null, StatusCodes.BlacklistedUser);
            }

            user.Status = UserStatus.Active.ToString();
            user.LastModifiedDate = DateTime.Now;
            user.EmailConfirmed = true;
            context.Update(user);
            await context.SaveChangesAsync();

            return ReturnedResponse.SuccessResponse("User successfully activated", null, StatusCodes.Successful);

        }

        public async Task<ApiResponse> CreateUser(SignUpRequestModel model)
        {
            var roles = await CreateRoles();
            if (!roles)
            {
                return ReturnedResponse.ErrorResponse("No roles found", null, StatusCodes.GeneralError);
            }
            var isUserExist = await _userManager.FindByEmailAsync(model.Email);

            if (isUserExist != null)
                return ReturnedResponse.ErrorResponse("User with this email already exists", null, StatusCodes.RecordExist);

            var resp = Luwa_sBackend.Core.Utility.ValidatePassword(model.Password);
            if (resp.Status == Status.UnSuccessful.ToString())
            {
                return resp;
            }

            var user = new ApplicationUser();
            user.Email = model.Email;
            user.UserName = model.Email;
            user.FirstName = model.Firstname;
            user.LastName = model.Lastname;
            user.PhoneNumber = Luwa_sBackend.Core.Utility.FormatPhoneNumber(model.PhoneNumber);
            user.Address = model.Address;


            user.Status = UserStatus.Inactive.ToString();
            user.Gender = model.Gender;

            user.LastModifiedDate = DateTime.Now;
            user.CreationDate = DateTime.Now;

            resp = await mailService.SendVerificationEmailAsync(user, OtpPurpose.UserVerification.ToString());

            if (resp.Status == Status.UnSuccessful.ToString())
                return resp;



            var x = await _userManager.CreateAsync(user, model.Password);
            await _userManager.AddToRoleAsync(user, UserRoles.User);
            
            var userDto = map.Map<UserDto>(user);
            return ReturnedResponse.SuccessResponse("User successfuly registered", userDto, StatusCodes.Successful);
        }


        public async Task<ApiResponse> CreatePartner([FromForm] SignUpPartnerRequestModel model)
        {
            var roles = await CreateRoles();
            if (!roles)
            {
                return ReturnedResponse.ErrorResponse("No roles found", null, StatusCodes.GeneralError);
            }
            var isUserExist = await _userManager.FindByEmailAsync(model.Email);

            if (isUserExist != null )
                return ReturnedResponse.ErrorResponse("User with this email already exists", null, StatusCodes.RecordExist);

            var resp = Luwa_sBackend.Core.Utility.ValidatePassword(model.Password);
            if (resp.Status == Status.UnSuccessful.ToString())
            {
                return resp;
            }

            var user = new ApplicationUser();
            user.Email = model.Email;
            user.UserName = model.Email;
            user.FirstName = model.Firstname;
            user.LastName = model.Lastname;
            user.PhoneNumber = Luwa_sBackend.Core.Utility.FormatPhoneNumber(model.PhoneNumber);
            user.Address = model.Address;


            user.Status = UserStatus.Inactive.ToString();
            user.Gender = model.Gender;

            user.LastModifiedDate = DateTime.Now;
            user.CreationDate = DateTime.Now;

            resp = await mailService.SendVerificationEmailAsync(user, OtpPurpose.UserVerification.ToString());

            if (resp.Status == Status.UnSuccessful.ToString())
                return resp;



            var x = await _userManager.CreateAsync(user, model.Password);
            await _userManager.AddToRoleAsync(user, UserRoles.Partners);
            

            string logoId = "";
            string ProfilePictureId = "";

            if(model.Logo != null)
            {
                var respp = await drive.UploadFileWithMetaData(model.Logo, user.Id, "logo");
                logoId = respp.Message;
            }


            if (model.ProfilePicture != null)
            {
                var respp = await drive.UploadFileWithMetaData(model.ProfilePicture, user.Id, "ProfilePicture");
                ProfilePictureId = respp.Message;
            }

            var partner = new Partner()
            {
                Description = model.Description,
                CreationDate = DateTime.Now,
                LastModifiedDate = DateTime.Now,
                Category = model.Category,
                UserId = user.Id,
                Rating = 0,
                Logo = logoId,
                ProfilePicture= ProfilePictureId
            };
            context.Add(partner);
            await context.SaveChangesAsync();
            var userDto = map.Map<UserDto>(user);
            return ReturnedResponse.SuccessResponse("Partner successfuly registered", userDto, StatusCodes.Successful);
        }

        public async Task<ApiResponse> DeActivateUser(string email)
        {
            // change the implementation do not delete user accoounts easily just make user inactive
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return ReturnedResponse.ErrorResponse("User not found", null, StatusCodes.NoRecordFound);
            if (user.Status == UserStatus.Blacklisted.ToString())
                return ReturnedResponse.ErrorResponse("Can't deactivate user as user is blacklisted", null, StatusCodes.BlacklistedUser);
            user.Status = UserStatus.Inactive.ToString();
            user.LastModifiedDate = DateTime.Now;
            context.Update(user);
            await context.SaveChangesAsync();

            return ReturnedResponse.SuccessResponse("User successfully deactivated", null, StatusCodes.Successful);
        }



        public async Task<ApiResponse> GetUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return ReturnedResponse.ErrorResponse("User with that email couldn't be found", null, StatusCodes.NoRecordFound);


            var userDto = map.Map<UserDto>(user);
            return ReturnedResponse.SuccessResponse("User found", userDto, StatusCodes.Successful);
        }

        public async Task<ApiResponse> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return ReturnedResponse.ErrorResponse("User with that email couldn't be found", null, StatusCodes.NoRecordFound);


            var userDto = map.Map<UserDto>(user);
            return ReturnedResponse.SuccessResponse("User found", userDto, StatusCodes.Successful);
        }

        public async Task<ApiResponse> GetUsers()
        {
            var users = await context.Users.ToListAsync();
            return ReturnedResponse.SuccessResponse("All users", users, StatusCodes.Successful);
        }

        public async Task<ApiResponse> GetUserRole(ApplicationUser user)
        {
            var userRole = await context.UserRoles.FirstOrDefaultAsync(x => x.UserId == user.Id);

            if (userRole == null)
                return ReturnedResponse.ErrorResponse("User doesn't have a role, contact admin", null, StatusCodes.GeneralError);

            var role = await context.Roles.FirstOrDefaultAsync(x => x.Id == userRole.RoleId);
            return ReturnedResponse.SuccessResponse("User Role", role, StatusCodes.Successful);
        }

        public async Task<ApiResponse> Login(LoginRequestModel model)
        {

            if (string.IsNullOrEmpty(model.EmailAddress))
                return ReturnedResponse.ErrorResponse("User Email can't be null or empty", null, StatusCodes.ModelError);
            var user = await _userManager.FindByEmailAsync(model.EmailAddress);

            if (user != null)
            {

                var isValid = await _signInManager.PasswordSignInAsync(user, model.Password, false, true);    //_userManager.PasswordHasher.VerifyHashedPassword(user, user.Password,model.Password);

                if (isValid.Succeeded)
                {


                    if (user.Status == UserStatus.Inactive.ToString() && user.EmailConfirmed == false)
                    {
                        _ = await mailService.SendVerificationEmailAsync(user, OtpPurpose.UserVerification.ToString());
                        return ReturnedResponse.SuccessResponse("Your account has not been verified, check your email to verify your account", null, StatusCodes.UnverifedUser);

                    }

                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles == null)
                    {
                        return ReturnedResponse.ErrorResponse("User doesn't have any role assigned yet. Contact admin", null, StatusCodes.NoRecordFound);
                    }



                    var authClaims = new List<Claim>
                    {
                      new Claim("userId", user.Id),

                    };

                    foreach (var userRole in roles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    }

                    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JwtSecret));

                    var token = new JwtSecurityToken(
                        issuer: _appSettings.ValidIssuer,
                        audience: _appSettings.ValidAudience,
                        expires: DateTime.Now.AddHours(_appSettings.JwtLifespan),
                        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
                        claims: authClaims
                        );

                    var bearerToken = new JwtSecurityTokenHandler().WriteToken(token);



                    var loginResponseModel = new LoginResponseModel
                    {
                        AccessToken = bearerToken,
                        TokenType = "Bearer",
                        Email = model.EmailAddress,
                        ExpiresIn = DateTime.Now.AddHours(_appSettings.JwtLifespan),
                    };


                    return ReturnedResponse.SuccessResponse("User Successfully logged in", loginResponseModel, StatusCodes.Successful);

                }

            }

            return ReturnedResponse.ErrorResponse("Invalid Email/Password", null, StatusCodes.GeneralError);

        }

        public async Task<ApiResponse> ForgotPasswordRequest(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return ReturnedResponse.ErrorResponse("User with this email not found", null, StatusCodes.NoRecordFound);

            }
            else
            {
                var resp = await mailService.SendForgotPasswordEmailAsync(user, OtpPurpose.PasswordReset.ToString());
                if (resp.Status == Status.Successful.ToString())
                    return ReturnedResponse.SuccessResponse("Your email has been sent to you to reset your password", null, StatusCodes.Successful);
                else
                    return ReturnedResponse.ErrorResponse("Couldn't send email to reset password", null, StatusCodes.GeneralError);
            }
        }



        public async Task<ApiResponse> ResetPassword(ChangePasswordRequestModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return ReturnedResponse.ErrorResponse("No Recond found", null, StatusCodes.NoRecordFound);
            }

            var resp = Luwa_sBackend.Core.Utility.ValidatePassword(model.Password);
            if (resp.Status == Status.UnSuccessful.ToString())
            {
                return resp;
            }

            await _userManager.RemovePasswordAsync(user);
            await _userManager.AddPasswordAsync(user, model.Password);

            return ReturnedResponse.SuccessResponse("Password has been reset", null, StatusCodes.Successful);
        }

        public async Task<ApiResponse> VerifyOtp(string otpCode)
        {
            var otp = await context.Otps.FirstOrDefaultAsync(x => x.OtpCode == otpCode);

            if (otp == null)
            {
                return ReturnedResponse.ErrorResponse("Invalid otp, input correct otp code", null, StatusCodes.GeneralError);

            }

            if (otp.ExpiryDate < DateTime.Now)
            {
                return ReturnedResponse.ErrorResponse("Otp has expired", null, StatusCodes.GeneralError);

            }


            if (otp.IsUsed == true)
            {
                return ReturnedResponse.ErrorResponse("Otp has expired", null, StatusCodes.GeneralError);

            }

            if (otp.Purpose == OtpPurpose.UserVerification.ToString())
            {
                var user = await _userManager.FindByEmailAsync(otp.Email);
                if (user == null)
                {
                    return ReturnedResponse.ErrorResponse("User account couldn't be verified", null, StatusCodes.NoRecordFound);
                }



                return await ActivateUser(user.Email);
            }

            else if (otp.Purpose == OtpPurpose.PasswordReset.ToString())
            {
                var user = await _userManager.FindByEmailAsync(otp.Email);
                if (user == null)
                {
                    return ReturnedResponse.ErrorResponse("User account couldn't be verified", null, StatusCodes.NoRecordFound);
                }

                return ReturnedResponse.SuccessResponse("User password reseted", null, StatusCodes.Successful);


            }

            else
            {
                return ReturnedResponse.ErrorResponse("An error occured while verifying otp", null, StatusCodes.GeneralError);
            }
        }

        public async Task<ApiResponse> ResendOTP(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return ReturnedResponse.ErrorResponse("user doesn't exists", null, StatusCodes.NoRecordFound);

            var isRegenerated = await otpService.ReSendOtpCode(email);

            if (isRegenerated)
            {
                var otp = await context.Otps.FirstOrDefaultAsync(x => x.Email == email);
                if (otp.Purpose == OtpPurpose.UserVerification.ToString())
                {
                    _ = await mailService.SendVerificationEmailAsync(user, otp.Purpose);
                }
                else
                    await mailService.SendForgotPasswordEmailAsync(user, otp.Purpose);

                return ReturnedResponse.SuccessResponse("otp code has been resent", null, StatusCodes.Successful);
            }

            return ReturnedResponse.ErrorResponse("an error occured", null, StatusCodes.GeneralError);
        }

        public Task<ApiResponse> UpdateUser()
        {
            throw new NotImplementedException();
        }
    }
}
