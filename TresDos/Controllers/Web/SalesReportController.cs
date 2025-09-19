using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SQLitePCL;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using TresDos.Application.DTOs.BetDto;
using TresDos.Application.DTOs.ProductDto;
using TresDos.Application.DTOs.Reports;
using TresDos.Application.ViewModel.BetModel;
using TresDos.Core.Entities;
using TresDos.Helper;
using TresDos.Services;

namespace TresDos.Controllers.Web
{
    [Route("SalesReport")]
    public class SalesReportController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        DateTimeHelper _dateTimeHelper = new DateTimeHelper();
        public SalesReportController(
            IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }
        private async Task ReferenceViewModel(SalesReportViewModel model)
        {
            var client = _clientFactory.CreateClient("ApiClient");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("JWToken"));

            var response = await client.GetAsync("api/usersapi");

            if (response.IsSuccessStatusCode)
            {
                model.Users = await response.Content.ReadFromJsonAsync<List<User>>() ?? new List<User>();
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                ViewBag.Error = error;
                model.Users = new List<User>();
            }

            model.Users.Insert(0, new User
            {
                Id = 0,           // or some special value representing "All"
                Username = "All"
            });
            model.DrawDate = _dateTimeHelper.GetPhilippineTime();
            model.SalesPerUser = new List<SalesReportResponseDTO>();
        }

        [HttpGet]
        public async Task<IActionResult> AllUsersByDate()
        {
            var model = new SalesReportViewModel();

            await ReferenceViewModel(model);

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> AllUsersByDate(SalesReportViewModel model)
        {
            await ReferenceViewModel(model);

            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
                return Json(new { error = "Unauthorized" });

            var client = _clientFactory.CreateClient("ApiClient");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("JWToken"));

            var response = await client.GetAsync(
                $"api/SalesReportApi/AllUsersByDate?drawDate={model.DrawDate.ToString("MM/dd/yyyy")}&UserId={model.UserId}"
            );

            if (response.IsSuccessStatusCode)
            {
                var winners = await response.Content.ReadFromJsonAsync<List<SalesReportResponseDTO>>();
                model.SalesPerUser = winners ?? new List<SalesReportResponseDTO>();
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                return Json(new { error });
            }

            return View(model);
        }
    }
}
