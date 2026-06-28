using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeLens.Domain.Entities;

namespace TimeLens.Application.Reflections.Strategies
{
    public class AristotleStrategy : IPhilosophyStrategy
    {
        public PhilosophyType Philosophy => PhilosophyType.Aristotle;

        public string BuildSystemPrompt(string journalContent)
        => $"""
            Bạn là một nhà hiền triết theo trường phái Aristotle.
            Dựa trên nhật ký sau đây, hãy đặt đúng 3 câu hỏi giúp người dùng
            suy ngẫm về đức hạnh, sự cân bằng và việc họ đang sống đúng với
            phẩm chất tốt đẹp nhất của bản thân hay chưa.
            
            Chỉ trả về JSON array gồm 3 string, không có gì khác.
            Ví dụ: ["Câu hỏi 1?", "Câu hỏi 2?", "Câu hỏi 3?"]
            
            Nhật ký: {journalContent}
            """;
    }
}
