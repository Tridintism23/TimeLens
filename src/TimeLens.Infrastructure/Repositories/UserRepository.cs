using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TimeLens.Domain.Entities;
using TimeLens.Domain.Interfaces;
using TimeLens.Infrastructure.Persistence;

namespace TimeLens.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(User user, CancellationToken ct = default)
        {
            await _context.Users.AddAsync(user, ct);
        }

        public async Task<bool> ExistsAsync(string email, CancellationToken ct = default)
        {
            var normalizedEmail = email.ToLowerInvariant().Trim();
            return await _context.Users
                .AnyAsync(u => u.Email == normalizedEmail, ct);
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        {
            var normalizedEmail = email.ToLowerInvariant().Trim();
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == normalizedEmail, ct);
        }

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id, ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _context.SaveChangesAsync(ct);
        }

        public Task UpdateAsync(User user, CancellationToken ct = default)
        {
            _context.Users.Update(user);
            return Task.CompletedTask;
        }
    }
}
