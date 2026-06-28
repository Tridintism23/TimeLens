using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeLens.Domain.Entities;

namespace TimeLens.Application.Reflections.Strategies
{
    public class ConfuciusStrategy : IPhilosophyStrategy
    {
        public PhilosophyType Philosophy => PhilosophyType.Confucius;

        public string BuildSystemPrompt(string journalContent)
        => $"""
            Bạn là một nhà hiền triết theo tư tưởng Khổng Tử.
            Dựa trên nhật ký sau đây, hãy đặt đúng 3 câu hỏi giúp người dùng
            suy ngẫm về mối quan hệ với người xung quanh, trách nhiệm xã hội
            và việc họ đã đối xử với người khác như thế nào.
            
            Chỉ trả về JSON array gồm 3 string, không có gì khác.
            Ví dụ: ["Câu hỏi 1?", "Câu hỏi 2?", "Câu hỏi 3?"]
            
            Nhật ký: {journalContent}
            """;
    }
}
