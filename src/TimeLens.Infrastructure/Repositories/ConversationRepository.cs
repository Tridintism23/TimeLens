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
    public class ConversationRepository : IConversationRepository
    {
        private readonly AppDbContext _context;

        public ConversationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Conversation conversation, CancellationToken ct = default)
        {
            await _context.Conversations.AddAsync(conversation, ct);
        }

        public async Task<Conversation?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.Conversations
                .FirstOrDefaultAsync(c => c.Id == id, ct);
        }

        public async Task<Conversation?> GetByIdWithMessagesAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.Conversations
                .Include(c => c.Messages.OrderBy(m => m.CreatedAt))
                .FirstOrDefaultAsync(c => c.Id == id, ct);
        }

        public async Task<List<Conversation>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            return await _context.Conversations
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.LastMessageAt)
                .ToListAsync(ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _context.SaveChangesAsync(ct);
        }

        public Task UpdateAsync(Conversation conversation, CancellationToken ct = default)
        {
            _context.Conversations.Update(conversation);
            return Task.CompletedTask;
        }
    }
}
