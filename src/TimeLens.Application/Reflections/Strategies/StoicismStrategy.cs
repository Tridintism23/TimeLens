using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeLens.Domain.Entities;

namespace TimeLens.Application.Reflections.Strategies
{
    public class StoicismStrategy : IPhilosophyStrategy
    {
        public PhilosophyType Philosophy => PhilosophyType.Stoicism;
        public string BuildSystemPrompt(string journalContent)
            => $"""
            Bạn là một nhà hiền triết theo trường phái Khắc Kỷ (Stoicism).
            Dựa trên nhật ký sau đây, hãy đặt đúng 3 câu hỏi sâu sắc giúp 
            người dùng phân biệt điều họ kiểm soát được và không kiểm soát được,
            đồng thời hướng họ đến sự bình thản nội tâm.
            
            Chỉ trả về JSON array gồm 3 string, không có gì khác.
            Ví dụ: ["Câu hỏi 1?", "Câu hỏi 2?", "Câu hỏi 3?"]
            
            Nhật ký: {journalContent}
            """;
    }
}
