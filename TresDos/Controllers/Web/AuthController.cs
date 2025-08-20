using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TresDos.Application.DTOs;
using TresDos.Application.DTOs.UserDto;
using TresDos.Application.ViewModel;

namespace TresDos.Controllers.Web
{
    public class AuthController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public AuthController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            ////for testing purposes only
            //model.Username = "ant";
            //model.Password = "testpassword";

            var client = _clientFactory.CreateClient("ApiClient");
            var response = await client.PostAsJsonAsync("/api/authapi/login", model);
            //var responseBody = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine(json);

                //var responseBody = await response.Content.ReadFromJsonAsync<TokenResponse>();
                var responseBody = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
                HttpContext.Session.SetString("JWToken", responseBody?.Token ?? "");
                HttpContext.Session.SetInt32("UserId", responseBody?.UserDetail.Id ?? 0);
                HttpContext.Session.SetString("UserFirstName", responseBody?.UserDetail.FirstName ?? string.Empty);
                HttpContext.Session.SetString("UserMiddleName", responseBody?.UserDetail.MiddleName ?? string.Empty);
                HttpContext.Session.SetString("UserLastName", responseBody?.UserDetail.LastName ?? string.Empty);
                HttpContext.Session.SetInt32("UserCommission", responseBody?.UserDetail.CommissionPercentage ?? 0);
                HttpContext.Session.SetInt32("UserParentId", responseBody?.UserDetail.ParentId ?? 0);

                return RedirectToAction("2d", "Bet");
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                //ViewBag.Error = response;
            }

            ViewBag.Error = "Invalid login";
            return View();
        }

        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            var client = _clientFactory.CreateClient("ApiClient");
            var response = await client.PostAsJsonAsync("api/authapi/register", model);

            return response.IsSuccessStatusCode ? RedirectToAction("Login") : View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Auth");
        }
    }

}
