using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeLens.Domain.Entities;

namespace TimeLens.Domain.Interfaces
{
    public interface IJournalRepository
    {
        Task<JournalEntry?> GetByIdAsync(Guid id, CancellationToken ct = default);

        Task<List<JournalEntry>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);

        Task<JournalEntry?> GetTodayEntryAsync(Guid userId, CancellationToken ct = default);

        Task<List<JournalEntry>> GetByDateRangeAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct = default);

        Task<List<JournalEntry>> GetEntriesWithoutTitleAsync(Guid userId, CancellationToken ct = default);

        Task AddAsync(JournalEntry entry, CancellationToken ct = default);

        Task UpdateAsync(JournalEntry entry, CancellationToken ct = default);

        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
