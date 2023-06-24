using AutoMapper;
using Luwa_sBackend.Data.Models.DTOs;
using LuwasBackend.Data.Entities;
using LuwasBackend.Data.Models.DTOs;
using System.Security.Principal;

namespace Luwa_sBackend.Core
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ApplicationUser, UserDto>();
            CreateMap<Chat, MessageDTO>();
           
        }
    }
}
