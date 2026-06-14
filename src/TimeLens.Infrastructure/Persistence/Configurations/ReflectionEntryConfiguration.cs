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
    public class ReflectionEntryConfiguration : IEntityTypeConfiguration<ReflectionEntry>
    {
        public void Configure(EntityTypeBuilder<ReflectionEntry> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Philosophy).HasConversion<string>().HasMaxLength(20);

            builder.Property(r => r.Questions).HasColumnType("nvarchar(max)");

            builder.Property(r => r.UserAnswer).HasColumnType("nvarchar(max)").IsRequired(false);

            builder.Property(r => r.Version).HasDefaultValue(1);

            builder.Property(r => r.IsActive).HasDefaultValue(true);

            builder.HasIndex(r => new { r.JournalEntryId, r.IsActive });

            builder.ToTable("ReflectionEntries");
        }
    }
}
