using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLens.Domain.Entities
{
    public enum PhilosophyType
    {
        Stoicism,   // Marcus Aurelius, Epictetus — kiểm soát bản thân
        Aristotle,  // Đức hạnh, sống cân bằng
        Confucius,  // Mối quan hệ, trách nhiệm xã hội
        Nietzsche   // Ý chí, sống theo bản ngã
    }
    public class ReflectionEntry
    {
        public const int MinVersion = 1;
        public Guid Id { get; private set; }
        public Guid JournalEntryId { get; private set; }
        public PhilosophyType Philosophy { get; private set; }         // "Stoicism" | "Aristotle" | "Confucius" | "Nietzsche"
        public string Questions { get; private set; } = string.Empty;          // JSON array 3 câu hỏi
        public string? UserAnswer { get; private set; }
        public int Version { get; private set; }                               // lần generate thứ mấy
        public bool IsActive { get; private set; }                             // chỉ 1 bản active tại một thời điểm
        public DateTime CreatedAt { get; private set; }
        public DateTime? AnsweredAt { get; private set; }

        private ReflectionEntry() { }

        // Factory method tạo ReflectionEntry
        public static ReflectionEntry Create(Guid journalEntryId, PhilosophyType philosophy, string questions, int version)
        {
            if (string.IsNullOrWhiteSpace(questions))
                throw new ArgumentException("Questions không được để trống.");
            if (version < MinVersion)
                throw new ArgumentOutOfRangeException(nameof(version), "Version phải >= " + MinVersion);

            return new ReflectionEntry()
            {
                Id = Guid.NewGuid(),
                JournalEntryId = journalEntryId,
                Philosophy = philosophy,
                Questions = questions,
                Version = version,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void RecordAnswer(string answer)
        {
            if (string.IsNullOrWhiteSpace(answer))
            {
                throw new ArgumentException("Câu trả lời không được để trống.");
            }
            UserAnswer = answer.Trim();
            AnsweredAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public bool IsAnswered() => !string.IsNullOrWhiteSpace(UserAnswer);
        public bool CanGenerate() => IsActive;
    }
}
