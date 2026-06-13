using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLens.Domain.Entities
{

    public enum MoodSource
    {
        UserInput,           // người dùng tự chọn
        AiAnalysis,          // AI tự phân tích từ nội dung nhật ký
        Both                 // Có cả 2 để so sánh
    }
    public class MoodEntry
    {
        public const int MisMatchLimit = 2;
        public const int MinScore = 1;
        public const int MaxScore = 5;
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public Guid? JournalEntryId { get; private set; }

        // Người dùng tự đánh giá
        public int? UserScore { get; private set; }
        public string? UserNote { get; private set; }

        // AI phân tích dựa trên nội dung nhật ký
        public int? AiScore { get; private set; }
        public string? AiReasoning { get; private set; }

        public MoodSource Source { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        private MoodEntry() { }

        // Factory method - tạo MoodEntry từ input người dùng
        public static MoodEntry CreateFromUser (Guid userId, int userScore, string? note = null, Guid? journalEntryId = null)
        {
            ValidateScore(userScore);
            return new MoodEntry()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                UserScore = userScore,
                UserNote = note?.Trim(),
                JournalEntryId = journalEntryId,
                Source = MoodSource.UserInput,
                CreatedAt = DateTime.UtcNow
            };
        }

        // Factory method - tạo MoodEntry từ AI phân tích 
        public static MoodEntry CreateFromAi(Guid userId, int aiScore, string reasoning, Guid? journalEntryId = null)
        {
            ValidateScore(aiScore);
            return new MoodEntry()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                AiScore = aiScore,
                AiReasoning = reasoning?.Trim(),
                JournalEntryId = journalEntryId,
                Source = MoodSource.AiAnalysis,
                CreatedAt = DateTime.UtcNow
            };
        }

        // Bổ sung AiScore và AiReasoning vào MoodEntry đã có UserScore, Source trở thành Both
        public void EnrichWithAiAnalysis(int aiScore, string reasoning)
        {
            ValidateScore(aiScore);
            AiScore = aiScore;
            AiReasoning = reasoning?.Trim();
            Source = MoodSource.Both;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateUserScore(int userScore, string? note = null)
        {
            ValidateScore(userScore);
            UserScore = userScore;
            UserNote = note?.Trim();
            UpdatedAt = DateTime.UtcNow;
        }

        // Kiểm tra xem liệu tâm trạng của người dùng có bị lệch nhiều so với AI phân tích hay không
        public bool HasMoodMismatch()
            => Source == MoodSource.Both && UserScore.HasValue && AiScore.HasValue && Math.Abs(UserScore.Value - AiScore.Value) >= MisMatchLimit;

        public int? EffectiveScore() => UserScore ?? AiScore;       // Ưu tiên score của người dùng
        private static void ValidateScore (int score)
        {
            if(score < MinScore || score > MaxScore)
            {
                throw new ArgumentOutOfRangeException(nameof(score), "Mood score phải từ " + MinScore + " đến " + MaxScore);
            }
        }
    }
}
