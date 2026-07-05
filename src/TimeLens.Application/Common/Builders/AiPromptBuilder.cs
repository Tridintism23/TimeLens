using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata;

namespace TimeLens.Application.Common.Builders
{
    public class AiPromptBuilder
    {
        private readonly StringBuilder _systemPrompt = new();
        private readonly StringBuilder _userMessage = new();
        private readonly List<string> _journalContext = new();
        private string? _philosophyInstruction;
        private int? _moodAverage;

        public AiPromptBuilder WithBaseInstruction(string instruction)
        {
            _systemPrompt.AppendLine(instruction);
            return this;
        }

        public AiPromptBuilder WithPhilosophy(string philosophyPrompt)
        {
            _philosophyInstruction = philosophyPrompt;
            return this;
        }

        public AiPromptBuilder WithJournalContext(IEnumerable<string> journals)
        {
            _journalContext.AddRange(journals);
            return this;
        }

        public AiPromptBuilder WithMoodContext(int averageMood)
        {
            _moodAverage = averageMood;
            return this;
        }

        public AiPromptBuilder WithUserMessage(string message)
        {
            _userMessage.AppendLine(message);
            return this;
        }

        public string BuildSystemPrompt()
        {
            var sb = new StringBuilder();

            if (_systemPrompt.Length > 0)
            {
                sb.AppendLine(_systemPrompt.ToString());
            }

            if (_philosophyInstruction is not null)
            {
                sb.AppendLine(_philosophyInstruction);
            }

            if (_journalContext.Count > 0)
            {
                sb.AppendLine("\nNhật ký của người dùng:");
                sb.AppendLine(string.Join("\n---\n", _journalContext));
            }

            if (_moodAverage.HasValue)
            {
                sb.AppendLine($"\nMood trung bình: {_moodAverage.Value}/5");
            }

            return sb.ToString();
        }

        public string BuildUserMessage()
            => _userMessage.ToString();
    }
}
