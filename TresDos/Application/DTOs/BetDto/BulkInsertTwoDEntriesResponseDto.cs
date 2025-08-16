using System.Collections.Generic;
using TresDos.Core.Entities;

namespace TresDos.Application.DTOs.BetDto
{
    public class BulkInsertTwoDEntriesResponseDto
    {
        public List<tb_TwoD> EntriesInserted { get; set; }
        public List<BulkInsertTwoDEntriesProcessingResultDto> EntriesWithError { get; set; }
    }
}
