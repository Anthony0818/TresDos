using Microsoft.EntityFrameworkCore.Metadata;
using TresDos.Core.Entities;

namespace TresDos.Services
{
    public interface ICacheTwoDValidAmount
    {
        Task<List<ltb_twoDValidAmounts>> GetDataAsync();
        Task RefreshCacheAsync();
    }
}
