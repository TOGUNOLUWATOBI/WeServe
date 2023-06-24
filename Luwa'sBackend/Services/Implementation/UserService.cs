using AutoMapper;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

using System.Collections.Generic;
using System.Security.Claims;
using Luwa_sBackend.Data.Models.ResponseModels;
using Luwa_sBackend.Data.ReturnedResponse;
using Luwa_sBackend.Data.Constants;
using Luwa_sBackend.Data.Models.RequestModels;
using Luwa_sBackend.Data;
using Luwa_sBackend.Services.Interfaces;
using Luwa_sBackend.Settings;
using Luwa_sBackend.Data.Models.DTOs;
using LuwasBackend.Data.Entities;
using LuwasBackend.Data.Constants;

namespace Luwa_sBackend.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext context;

        public UserService(ApplicationDbContext context)
        {
            this.context = context;
        }


    }
}
