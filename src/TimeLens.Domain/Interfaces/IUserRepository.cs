using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeLens.Domain.Entities;

namespace TimeLens.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync (Guid id, CancellationToken ct = default);

        Task<User?> GetByEmailAsync (string email, CancellationToken ct = default);

        Task<bool> ExistsAsync (string email, CancellationToken ct = default);

        Task AddAsync (User user, CancellationToken ct = default);

        Task UpdateAsync (User user, CancellationToken ct = default);

        Task SaveChangesAsync (CancellationToken ct = default);
    }
}
