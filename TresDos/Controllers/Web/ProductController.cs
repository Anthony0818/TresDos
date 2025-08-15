using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using TresDos.Core.Entities;

namespace TresDos.Controllers.Web
{
    public class ProductController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        public ProductController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _clientFactory.CreateClient("ApiClient");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("JWToken"));

            var response = await client.GetAsync("api/products");
            if (!response.IsSuccessStatusCode)
            {
                // Optionally: log error details or return error view
                ViewBag.Error = "Unable to load products.";
                return View(new List<Product>());
            }
            var products = await response.Content.ReadFromJsonAsync<List<Product>>();

            return View(products);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            var client = _clientFactory.CreateClient("ApiClient");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("JWToken"));

            var response = await client.PostAsJsonAsync("api/products", product);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            var error = await response.Content.ReadAsStringAsync();
            ViewBag.Error = $"Failed to save product: {error}";
            return View(product);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var client = _clientFactory.CreateClient("ApiClient");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("JWToken"));

            var response = await client.GetAsync($"api/products/{id}");
            var product = await response.Content.ReadFromJsonAsync<Product>();

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Product product)
        {
            var client = _clientFactory.CreateClient("ApiClient");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("JWToken"));

            await client.PutAsJsonAsync($"api/products/{product.Id}", product);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var client = _clientFactory.CreateClient("ApiClient");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("JWToken"));

            await client.DeleteAsync($"api/products/{id}");
            return RedirectToAction("Index");
        }
    }
}
