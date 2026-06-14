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
    public class JournalEntryConfiguration : IEntityTypeConfiguration<JournalEntry>
    {
        public void Configure(EntityTypeBuilder<JournalEntry> builder)
        {
            builder.HasKey(j => j.Id);

            builder.Property(j => j.Title).HasMaxLength(200);

            builder.Property(j => j.Content).HasColumnType("nvarchar(max)");

            builder.Property(j => j.IsTitleAiGenerated).HasDefaultValue(false);

            builder.HasOne(j => j.MoodEntry).WithOne().HasForeignKey<MoodEntry>(m => m.JournalEntryId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(j => j.ReflectionEntries).WithOne().HasForeignKey(r => r.JournalEntryId).OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("JournalEntries");
        }
    }
}
