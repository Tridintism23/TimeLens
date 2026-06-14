using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TimeLens.Domain.Entities;
using TimeLens.Domain.Interfaces;
using TimeLens.Infrastructure.Persistence;

namespace TimeLens.Infrastructure.Repositories
{
    public class ReflectionEntryRepository : IReflectionRepository
    {
        private readonly AppDbContext _context;

        public ReflectionEntryRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddSync(ReflectionEntry entry, CancellationToken ct = default)
        {
            await _context.ReflectionEntries.AddAsync(entry, ct);
        }

        public async Task<ReflectionEntry?> GetActiveByJournalEntryIdAsync(Guid journalEntryId, CancellationToken ct = default)
        {
            return await _context.ReflectionEntries
                .FirstOrDefaultAsync(r => r.JournalEntryId == journalEntryId && r.IsActive, ct);
        }

        public async Task<List<ReflectionEntry>> GetAllByJournalEntryIdAsync(Guid journalEntryId, CancellationToken ct = default)
        {
            return await _context.ReflectionEntries
                .Where(r => r.JournalEntryId == journalEntryId)
                .OrderBy(r => r.Version)
                .ToListAsync(ct);
        }

        public async Task<int> GetLatestVersionAsync(Guid journalEntryId, CancellationToken ct = default)
        {
            var versions = await _context.ReflectionEntries
                .Where(r => r.JournalEntryId == journalEntryId)
                .Select(r => r.Version)
                .ToListAsync(ct);
            return versions.Count == 0 ? 0 : versions.Max();
        }

        public async Task<List<ReflectionEntry>> GetUnansweredByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            return await _context.ReflectionEntries
                .Where(r => r.IsActive && r.UserAnswer == null)
                .Join(_context.JournalEntries
                .Where(j => j.UserId == userId), r => r.JournalEntryId, j => j.Id, (r, j) => r)
                .ToListAsync(ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _context.SaveChangesAsync(ct);
        }

        public Task UpdateAsync(ReflectionEntry entry, CancellationToken ct = default)
        {
            _context.ReflectionEntries.Update(entry);
            return Task.CompletedTask;
        }
    }
}
