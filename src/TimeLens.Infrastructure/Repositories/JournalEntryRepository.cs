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
    public class JournalEntryRepository : IJournalRepository
    {
        private readonly AppDbContext _context;

        public JournalEntryRepository (AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(JournalEntry entry, CancellationToken ct = default)
        {
            await _context.JournalEntries.AddAsync(entry, ct);
        }

        public async Task<List<JournalEntry>> GetByDateRangeAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct = default)
        {
            return await _context.JournalEntries
                .Where(j => j.UserId == userId && j.CreatedAt >= from && j.CreatedAt <= to)
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync(ct);
        }

        public async Task<JournalEntry?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.JournalEntries
                .Include(j => j.MoodEntry)
                .Include(j => j.ReflectionEntries)
                .FirstOrDefaultAsync(j => j.Id == id, ct);
        }

        public async Task<List<JournalEntry>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            return await _context.JournalEntries
                .Where(j => j.UserId == userId)
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync(ct);
        }

        public async Task<List<JournalEntry>> GetEntriesWithoutTitleAsync(Guid userId, CancellationToken ct = default)
        {
            return await _context.JournalEntries
                .Where(j => j.UserId == userId && j.Title == string.Empty)
                .ToListAsync(ct);
        }

        public async Task<JournalEntry?> GetTodayEntryAsync(Guid userId, CancellationToken ct = default)
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            return await _context.JournalEntries
                .FirstOrDefaultAsync(j => j.UserId == userId && j.CreatedAt >= today && j.CreatedAt < tomorrow, ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _context.SaveChangesAsync(ct);
        }

        public Task UpdateAsync(JournalEntry entry, CancellationToken ct = default)
        {
            _context.JournalEntries.Update(entry);
            return Task.CompletedTask;
        }
    }
}
