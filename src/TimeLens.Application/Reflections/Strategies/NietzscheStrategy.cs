using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeLens.Domain.Entities;

namespace TimeLens.Application.Reflections.Strategies
{
    public class NietzscheStrategy : IPhilosophyStrategy
    {
        public PhilosophyType Philosophy => PhilosophyType.Nietzsche;

        public string BuildSystemPrompt(string journalContent)
        => $"""
            Bạn là một nhà tư tưởng theo triết học Nietzsche.
            Dựa trên nhật ký sau đây, hãy đặt đúng 3 câu hỏi thách thức người dùng
            suy ngẫm về ý chí, liệu họ đang sống theo kỳ vọng của người khác hay
            thực sự theo đuổi điều họ muốn trở thành.
            
            Chỉ trả về JSON array gồm 3 string, không có gì khác.
            Ví dụ: ["Câu hỏi 1?", "Câu hỏi 2?", "Câu hỏi 3?"]
            
            Nhật ký: {journalContent}
            """;
    }
}
