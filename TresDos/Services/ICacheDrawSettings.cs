using Microsoft.EntityFrameworkCore.Metadata;
using TresDos.Core.Entities;

namespace TresDos.Services
{
    public interface ICacheDrawSettings
    {
        Task<List<ltb_DrawSettings>> GetDataAsync();
        Task RefreshCacheAsync();
    }
}
