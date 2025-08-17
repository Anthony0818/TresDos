using TresDos.Core.Entities;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;

namespace TresDos.Services
{
    public class CacheTwoDValidaAmount : ICacheDrawSettings
    {
        private List<ltb_twoDValidAmounts> _cachedData = new();
        private readonly IHttpClientFactory _clientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CacheTwoDValidaAmount(
            IHttpClientFactory clientFactory,
            IHttpContextAccessor httpContextAccessor)
        {
            _clientFactory = clientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<ltb_twoDValidAmounts>> GetDataAsync()
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
                _cachedData = new List<ltb_twoDValidAmounts>();
                return;
            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("api/DrawSettingsApi");

            if (response.IsSuccessStatusCode)
            {
                _cachedData = await response.Content.ReadFromJsonAsync<List<ltb_twoDValidAmounts>>()
                              ?? new List<ltb_twoDValidAmounts>();
            }
            else
            {
                _cachedData = new List<ltb_twoDValidAmounts>();
            }
        }
    }
}
