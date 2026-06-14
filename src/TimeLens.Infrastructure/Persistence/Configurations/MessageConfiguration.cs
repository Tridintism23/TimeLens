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
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.HasKey(m  => m.Id);

            builder.Property(m => m.Role).HasConversion<string>().HasMaxLength(20);

            builder.Property(m => m.Content).HasColumnType("nvarchar(max)");

            builder.HasIndex(m => new { m.ConversationId, m.CreatedAt });

            builder.ToTable("Messages");
        }
    }
}
