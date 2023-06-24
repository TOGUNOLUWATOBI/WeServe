using Luwa_sBackend.Data;
using Luwa_sBackend.Data.Constants;
using Luwa_sBackend.Data.Models.DTOs;
using Luwa_sBackend.Data.ReturnedResponse;
using LuwasBackend.Data.Entities;
using LuwasBackend.Data.Enums;
using LuwasBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuwasBackend.Services.Implementation
{
    public class DashBoardService : IDashBoardService
    {
        private readonly ApplicationDbContext context;

        public DashBoardService(ApplicationDbContext context)
        {
            this.context = context;
        }


        public async Task<ApiResponse> GetPartnersByCategory(string category, int pageNumber = 1, int pageSize = 10, string? filter = null)
        {
            try
            {
                if (pageNumber < 1)
                {
                    pageNumber = 1;
                }
                if (pageSize < 1)
                {
                    pageSize = 10;
                }

                int skipCount = (pageNumber - 1) * pageSize;

                if (!Enum.GetNames(typeof(Categories)).Contains(category))
                    return ReturnedResponse.ErrorResponse($"Categry type doesn't exist: {category}", null,StatusCodes.ModelError);

                var parnters = await context.Partners.Where(x=> x.Category == category).ToListAsync();

                if (filter != null && filter == PartnerFilters.RatingDesc.ToString())
                {
                    parnters = parnters.OrderByDescending(x => x.Rating).ToList();
                }

                if (filter != null && filter == PartnerFilters.RatingAsc.ToString())
                {
                    parnters = parnters.OrderBy(x => x.Rating).ToList();
                }
                var result = parnters.Skip(skipCount).Take(pageSize).ToList();

                return ReturnedResponse.SuccessResponse("Partners retrieved successfully", result,StatusCodes.Successful);
            }
            catch(Exception ex)
            {
                return ReturnedResponse.ErrorResponse($"An error occured: {ex.Message}", null,StatusCodes.ExceptionError);
            }
        }


        public async Task<ApiResponse> Search (string keyword)
        {
            var user = await context.Users.Where(x=> x.FirstName.Contains(keyword) || x.LastName.Contains(keyword)).Select(x => x.Id).ToListAsync();

            var parnters = await context.Partners.Where(x => x.Category.Contains(keyword)  || x.Description.Contains(keyword) || user.Contains(x.UserId)).ToListAsync();

            return ReturnedResponse.SuccessResponse("Search result", parnters,StatusCodes.Successful);

        }

        public async Task<ApiResponse> GetDashboardHome()
        {
            try
            {
                var resp1 = await GetPartnersByCategory(Categories.programming_and_tech.ToString());
                var resp2 = await GetPartnersByCategory(Categories.writing.ToString());
                var resp3 = await GetPartnersByCategory(Categories.graphics_design.ToString());
                var resp4 = await GetPartnersByCategory(Categories.hairstyling.ToString());



                var partnerByCat1 = (List<Partner>) resp1.Data;
                var partnerByCat2 = (List<Partner>) resp2.Data;
                var partnerByCat3 = (List<Partner>) resp3.Data;
                var partnerByCat4 = (List<Partner>) resp4.Data;

                var grouped = new List<Dashboardpartners>
                {
                    new Dashboardpartners() { Name = Categories.programming_and_tech.ToString(), Parnters = partnerByCat1 },
                    new Dashboardpartners() { Name = Categories.writing.ToString(), Parnters = partnerByCat2 },
                    new Dashboardpartners() { Name = Categories.graphics_design.ToString(), Parnters = partnerByCat3 },
                    new Dashboardpartners() { Name = Categories.hairstyling.ToString(), Parnters = partnerByCat4 }
                };

                return ReturnedResponse.SuccessResponse("Dashboard successfully generated", grouped, StatusCodes.Successful);
            }
            catch (Exception ex)
            {
                return ReturnedResponse.ErrorResponse($"An error occured: {ex.Message}", null, StatusCodes.ExceptionError);
            }
        }
    }

    }
