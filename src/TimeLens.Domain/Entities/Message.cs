using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLens.Domain.Entities
{
    public enum MessageRole
    {
        User,              // Tin nhắn từ người dùng
        Assistant          // Tin nhắn từ AI
    }
    public class Message
    {
        public Guid Id { get; private set; }
        public Guid ConversationId { get; private set; }
        public MessageRole Role { get; private set; }
        public string Content { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; }

        private Message() { }

        // Factory method để tạo Message
        public static Message CreateUserMessage(Guid conversationId, string content)
        {
            ValidateContent(content);
            return new Message
            {
                Id = Guid.NewGuid(),
                ConversationId = conversationId,
                Role = MessageRole.User,
                Content = content.Trim(),
                CreatedAt = DateTime.UtcNow
            };  
        }

        // Factory method để tạo Message
        public static Message CreateAssistantMessage(Guid conversationId, string content) {
            ValidateContent(content);
            return new Message {
                Id = Guid.NewGuid(),
                ConversationId = conversationId,
                Role = MessageRole.Assistant,
                Content = content.Trim(),
                CreatedAt = DateTime.UtcNow
            };
        }

        private static void ValidateContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentException("Nội dung tin nhắn không được để trống.");
            }
        }
    }
}
