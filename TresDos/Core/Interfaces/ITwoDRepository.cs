using Microsoft.EntityFrameworkCore;
using TresDos.Application.DTOs.BetDto;
using TresDos.Core.Entities;

namespace TresDos.Core.Interfaces
{
    public interface ITwoDRepository
    {
        Task<IEnumerable<tb_TwoD>> GetAllAsync();
        Task<tb_TwoD?> GetByIdAsync(int id);
        Task AddAsync(tb_TwoD entry);
        Task UpdateAsync(tb_TwoD entry);
        Task DeleteAsync(int id);
        //Task<decimal> GetRAMBLECurrentTotalAsync(
        //    string typeCode,
        //    int firstDigit,
        //    int secondDigit);
        //Task<decimal> GetSTRAIGHTCurrentTotalAsync(string typeCode,
        //    int firstDigit,
        //    int secondDigit);
        Task<decimal> GetCurrentTotalAsync(
             string typeCode,
             int firstDigit,
             int secondDigit,
             string drawType,
             DateTime drawDate
            );
        Task AddEntriesAsync(IEnumerable<tb_TwoD> entries);
        Task<bool> EntryExistsAsync(
            string bettor,
            int firstDigit,
            int secondDigit,
            decimal amount,
            string type,
            string drawType,
            DateTime drawDate);
        Task<List<TwoDBetsDto>?> GetBetsByUserIdDrawTypeDrawDate(int userId, string drawType, DateTime drawDate);
      }
}
