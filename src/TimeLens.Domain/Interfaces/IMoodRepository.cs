using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeLens.Domain.Entities;

namespace TimeLens.Domain.Interfaces
{
    public interface IMoodRepository
    {
        Task<MoodEntry?> GetByJournalEntryIdAsync (Guid journalEntryId, CancellationToken ct = default);

        Task<MoodEntry?> GetTodayMoodAsync (Guid userId, CancellationToken ct = default);

        Task<List<MoodEntry>> GetByUserIdAsync (Guid userId, CancellationToken ct = default);

        Task<List<MoodEntry>> GetByDateRangeAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct = default);

        Task<List<MoodEntry>> GetMismatchedMoodsAsync (Guid userId, CancellationToken ct = default);

        Task AddAsync (MoodEntry entry, CancellationToken ct = default);

        Task UpdateAsync (MoodEntry entry, CancellationToken ct = default);

        Task SaveChangesAsync (CancellationToken ct = default);
    }
}
