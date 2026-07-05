using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLens.Domain.Entities
{
    public class Conversation
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; }
        public DateTime LastMessageAt { get; private set; }

        public ICollection<Message> Messages { get; private set; } = new List<Message>();

        private Conversation() { }

        // Factory method tạo Conversation
        public static Conversation Create(Guid userId, string? title = null)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("UserId không hợp lệ.");
            }
            return new Conversation
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = string.IsNullOrWhiteSpace(title) ? "Cuộc trò chuyện mới" : title.Trim(),
                CreatedAt = DateTime.UtcNow,
                LastMessageAt = DateTime.UtcNow
            };
        }

        public void UpdateTitle (string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Tiêu đề không được để trống.");
            }
            Title = title.Trim();
        }

        public void UpdateLastMessageTime()
        {
            LastMessageAt = DateTime.UtcNow;
        }

        public bool HasMessages() => Messages.Any();
    }
}
