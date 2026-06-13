using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeLens.Domain.Entities;

namespace TimeLens.Domain.Interfaces
{
    public interface IReflectionRepository
    {
        Task<ReflectionEntry?> GetActiveByJournalEntryIdAsync (Guid journalEntryId, CancellationToken ct = default);

        Task<List<ReflectionEntry>> GetAllByJournalEntryIdAsync(Guid journalEntryId, CancellationToken ct = default);

        Task<int> GetLatestVersionAsync(Guid journalEntryId, CancellationToken ct = default);

        Task<List<ReflectionEntry>> GetUnansweredByUserIdAsync (Guid userId, CancellationToken ct = default);

        Task AddSync (ReflectionEntry entry, CancellationToken ct = default);

        Task UpdateAsync (ReflectionEntry entry, CancellationToken ct = default);

        Task SaveChangesAsync (CancellationToken ct = default);
    }
}
