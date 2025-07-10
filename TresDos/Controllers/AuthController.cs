using Microsoft.AspNetCore.Mvc;
using TresDos.Models;

namespace TresDos.Controllers
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
            var response = await client.PostAsJsonAsync("api/auth/login", model);

            if (response.IsSuccessStatusCode)
            {
                //var token = await response.Content.ReadAsStringAsync();
                //string s = token.Trim('"');
                //HttpContext.Session.SetString("JWToken", token.Trim('"'));
                //return RedirectToAction("Index", "Product");
                var responseBody = await response.Content.ReadFromJsonAsync<TokenResponse>();
                HttpContext.Session.SetString("JWToken", responseBody?.Token ?? "");
                return RedirectToAction("Index", "Product");
            }

            ViewBag.Error = "Invalid login";
            return View();
        }

        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            var client = _clientFactory.CreateClient("ApiClient");
            var response = await client.PostAsJsonAsync("api/auth/register", model);

            return response.IsSuccessStatusCode ? RedirectToAction("Login") : View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }

}
