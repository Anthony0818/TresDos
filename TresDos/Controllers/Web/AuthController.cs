using Microsoft.AspNetCore.Mvc;
using TresDos.Application.DTOs;

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
            var client = _clientFactory.CreateClient("ApiClient");
            var response = await client.PostAsJsonAsync("/api/authapi/login", model);
            var responseBody = await response.Content.ReadAsStringAsync();
            //if (response.IsSuccessStatusCode)
            //{
            //    var responseBody = await response.Content.ReadFromJsonAsync<TokenResponse>();
            //    HttpContext.Session.SetString("JWToken", responseBody?.Token ?? "");
            //    return RedirectToAction("2d", "Bet");
            //}

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
            return RedirectToAction("Login");
        }
    }

}
