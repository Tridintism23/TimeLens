using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeLens.Domain.Entities;

namespace TimeLens.Domain.Interfaces
{
    public interface IConversationRepository
    {
        Task<Conversation?> GetByIdAsync(Guid id, CancellationToken ct = default);

        Task<Conversation?> GetByIdWithMessagesAsync (Guid id, CancellationToken ct = default);

        Task<List<Conversation>> GetByUserIdAsync (Guid userId, CancellationToken ct = default);

        Task AddAsync(Conversation conversation, CancellationToken ct = default);

        Task UpdateAsync (Conversation conversation, CancellationToken ct = default);

        Task SaveChangesAsync (CancellationToken ct = default);
    }
}
