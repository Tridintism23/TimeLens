using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeLens.Domain.Entities;

namespace TimeLens.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Email).IsRequired().HasMaxLength(256);

            builder.Property(u => u.FullName).IsRequired().HasMaxLength(100);

            builder.Property(u => u.PasswordHash).IsRequired();

            builder.HasIndex(u => u.Email).IsUnique();

            builder.Property(u => u.PreferredPhilosophy).HasConversion<string>().HasMaxLength(20);

            builder.HasMany(u => u.JournalEntries).WithOne().HasForeignKey(j => j.UserId).OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.MoodEntries).WithOne().HasForeignKey(m => m.UserId).OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(u => u.Conversations).WithOne().HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("Users");
        }
    }
}
