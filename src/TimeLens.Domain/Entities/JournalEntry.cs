using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLens.Domain.Entities
{
    public class JournalEntry
    {
        public Guid Id { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public bool IsTitleAiGenerated { get; private set; }
        public string Content { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }           // track chỉnh sửa
        public Guid UserId { get; private set; }

        // Navigation properties cho EF Core
        public MoodEntry? MoodEntry { get; private set; }
        public ICollection<ReflectionEntry> ReflectionEntries { get; private set; } = new List<ReflectionEntry>();

        private JournalEntry() { }

        // Factory method tạo JournalEntry
        public static JournalEntry Create(string? title, string content, Guid userId)
        {
            return new JournalEntry()
            {
                Id = Guid.NewGuid(),
                Title = title?.Trim() ?? string.Empty,
                IsTitleAiGenerated = false,
                Content = content?.Trim() ?? string.Empty,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };
        }

        // Behavior method — logic nằm trong Entity, không nằm rải rác ở Service
        public void Update(string title, string content)
        {
            Title = title?.Trim() ?? string.Empty;
            Content = content?.Trim() ?? string.Empty;
            IsTitleAiGenerated = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetAiGeneratedTitle(string aiTitle)
        {
            if (string.IsNullOrWhiteSpace(aiTitle))
            {
                throw new ArgumentException("AI Title không được để trống.");
            }
            Title = aiTitle.Trim();
            IsTitleAiGenerated = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public bool HasContent() => !string.IsNullOrWhiteSpace(Content);
        public bool HasTitle() => !string.IsNullOrWhiteSpace(Title);
        public bool WasEdited() => UpdatedAt.HasValue;

        public bool WasEditedToday()
            => UpdatedAt.HasValue && UpdatedAt.Value.Date == DateTime.UtcNow.Date;


    }
}
