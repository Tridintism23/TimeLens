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
    public class MoodEntryRepository : IMoodRepository
    {
        private readonly AppDbContext _context;

        public MoodEntryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(MoodEntry entry, CancellationToken ct = default)
        {
            await _context.MoodEntries.AddAsync(entry, ct);
        }

        public async Task<List<MoodEntry>> GetByDateRangeAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct = default)
        {
            return await _context.MoodEntries
                .Where(m => m.UserId == userId && m.CreatedAt >= from && m.CreatedAt <= to)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync(ct);
        }

        public async Task<MoodEntry?> GetByJournalEntryIdAsync(Guid journalEntryId, CancellationToken ct = default)
        {
            return await _context.MoodEntries
                .FirstOrDefaultAsync(m => m.JournalEntryId == journalEntryId, ct);
        }

        public async Task<List<MoodEntry>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            return await _context.MoodEntries
                .Where(m => m.UserId == userId)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync(ct);
        }

        public async Task<List<MoodEntry>> GetMismatchedMoodsAsync(Guid userId, CancellationToken ct = default)
        {
            var moods = await _context.MoodEntries
                .Where(m => m.UserId == userId && m.Source == MoodSource.Both)
                .ToListAsync(ct);
            return moods.Where(m => m.HasMoodMismatch()).ToList();
        }

        public async Task<MoodEntry?> GetTodayMoodAsync(Guid userId, CancellationToken ct = default)
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            return await _context.MoodEntries
                .FirstOrDefaultAsync(m => m.UserId == userId && m.CreatedAt >= today && m.CreatedAt < tomorrow, ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _context.SaveChangesAsync(ct);
        }

        public Task UpdateAsync(MoodEntry entry, CancellationToken ct = default)
        {
            _context.MoodEntries.Update(entry);
            return Task.CompletedTask;
        }
    }
}
