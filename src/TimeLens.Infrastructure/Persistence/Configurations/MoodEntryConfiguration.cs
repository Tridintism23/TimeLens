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
    public class MoodEntryConfiguration : IEntityTypeConfiguration<MoodEntry>
    {
        public void Configure(EntityTypeBuilder<MoodEntry> builder)
        {
            builder.HasKey(m  => m.Id);

            builder.Property(m => m.UserScore).IsRequired(false);

            builder.Property(m => m.AiScore).IsRequired(false);

            builder.Property(m => m.UserNote).HasMaxLength(500).IsRequired(false);

            builder.Property(m => m.AiReasoning).HasMaxLength(1000).IsRequired(false);

            builder.Property(m => m.Source).HasConversion<string>().HasMaxLength(20);

            builder.ToTable("MoodEntries");
        }
    }
}
