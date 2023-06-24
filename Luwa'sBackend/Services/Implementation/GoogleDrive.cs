﻿using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using Luwa_sBackend.Data;
using Luwa_sBackend.Data.ReturnedResponse;
using LuwasBackend.Data.Entities;
using LuwasBackend.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using StatusCodes = Luwa_sBackend.Data.Constants.StatusCodes;

namespace LuwasBackend.Services.Implementation
{
    public class GoogleDrive : IGoogleDrive
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext context;

        public GoogleDrive(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            this.userManager = userManager;
            this.context = context;
        }

        private const string DirectoryId = "1k5Wqgbplnkls7C9Of6R2acf00U-eIz5o";
        // private const string uploadFilenme = "testing";
        private const string serviceAccountEmail = "fypbackend@fypbackend2023.iam.gserviceaccount.com";
        //private const string filePath = @"C:\Users\BEBS\Pictures\passport(2).jpg";
        public async Task<ApiResponse> UploadFileWithMetaData(IFormFile requestFile, string userId, string ImageType)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return ReturnedResponse.ErrorResponse("user not found", null, StatusCodes.NoRecordFound);
            }
            if (requestFile == null)
                ReturnedResponse.ErrorResponse("the file has a issue being found to be uploaded", null, StatusCodes.GeneralError);
            try
            {

                var certificate = new X509Certificate2("key.p12", "notasecret", X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable);


                var credential = new ServiceAccountCredential(
               new ServiceAccountCredential.Initializer(serviceAccountEmail)
               {
                   Scopes = new[] { DriveService.Scope.Drive, DriveService.Scope.DriveFile, DriveService.Scope.DriveAppdata }
               }.FromCertificate(certificate));

                var service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "FYPBackEnd",
                });
                Console.WriteLine("passed 2 stage");
                var fileMetaData = new Google.Apis.Drive.v3.Data.File()
                {


                    Name = user.FirstName + " " + user.LastName + ImageType,
                    MimeType = "image/jpeg",
                    Parents = new[] { DirectoryId }
                };
                Console.WriteLine("passed 3 stage");

                // Create a new file on drive.
                await using (var stream = requestFile.OpenReadStream())
                {
                    // Create a new file, with metadata and stream.
                    var request = service.Files.Create(fileMetaData, stream, "image/jpeg");

                    Console.WriteLine("passed 4 stage");
                    //request.Fields = "id";
                    var result = await request.UploadAsync(CancellationToken.None);

                    if (result.Status == UploadStatus.Failed)
                    {
                        return ReturnedResponse.ErrorResponse(null, null, StatusCodes.GeneralError);
                    }
                    var file = request.ResponseBody;
                  
                    //todo: Change this implementation so as to make sure that it is used to save files and pictures

                    //user.ProficePictureId = file.Id;
                    //context.Update(user);
                    await context.SaveChangesAsync();
                    return ReturnedResponse.SuccessResponse(file.Id, file, StatusCodes.Successful);
                }


            }
            catch (Exception ex)
            {
                return ReturnedResponse.ErrorResponse(ex.Message, ex.InnerException.ToString(), StatusCodes.ExceptionError);
            }
        }
    }
}