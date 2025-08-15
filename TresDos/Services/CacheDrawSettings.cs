using TresDos.Core.Entities;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;

namespace TresDos.Services
{
    public class CacheDrawSettings : ICacheDrawSettings
    {
        private List<ltb_DrawSettings> _cachedData = new();
        private readonly IHttpClientFactory _clientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CacheDrawSettings(
            IHttpClientFactory clientFactory,
            IHttpContextAccessor httpContextAccessor)
        {
            _clientFactory = clientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<ltb_DrawSettings>> GetDataAsync()
        {
            // Optionally, auto-refresh if cache is empty
            if (_cachedData == null || !_cachedData.Any())
            {
                await RefreshCacheAsync();
            }

            return _cachedData;
        }

        public async Task RefreshCacheAsync()
        {
            var client = _clientFactory.CreateClient("ApiClient");

            // Get JWT token from session (safely)
            var context = _httpContextAccessor.HttpContext;
            var token = context?.Session?.GetString("JWToken");

            if (string.IsNullOrEmpty(token))
            {
                // Optional: throw or log warning
                _cachedData = new List<ltb_DrawSettings>();
                return;
            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("api/DrawSettingsApi");

            if (response.IsSuccessStatusCode)
            {
                _cachedData = await response.Content.ReadFromJsonAsync<List<ltb_DrawSettings>>()
                              ?? new List<ltb_DrawSettings>();
            }
            else
            {
                _cachedData = new List<ltb_DrawSettings>();
            }
        }
    }
}
